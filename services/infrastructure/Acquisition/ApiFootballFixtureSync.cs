using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Mis.Application.Acquisition;
using Mis.Domain.Acquisition;
using Mis.Domain.Football;
using Mis.Infrastructure.Persistence;

namespace Mis.Infrastructure.Acquisition;

public sealed class ApiFootballFixtureSync(HttpClient client, MisDbContext database) : IManualFixtureSync
{
    public async Task<ManualFixtureSyncResult> ExecuteAsync(CancellationToken cancellationToken)
    {
        var run = new SyncRun(Guid.NewGuid(), "api-football", DateTimeOffset.UtcNow);
        database.SyncRuns.Add(run);
        await database.SaveChangesAsync(cancellationToken);

        try
        {
            var items = (await GetAsync("next", cancellationToken)).Concat(await GetAsync("last", cancellationToken))
                .GroupBy(item => item.GetProperty("fixture").GetProperty("id").GetInt32()).Select(group => group.First()).ToArray();
            var mappings = await database.ProviderEntityMappings.Where(mapping => mapping.Provider == "api-football").ToListAsync(cancellationToken);
            var mappedIds = mappings.ToDictionary(mapping => $"{mapping.ResourceType}:{mapping.ExternalId}", mapping => mapping.CanonicalId);
            var teams = await database.Teams.ToDictionaryAsync(team => team.Id, cancellationToken);
            var competitions = await database.Competitions.ToDictionaryAsync(competition => competition.Id, cancellationToken);
            var seasons = await database.Seasons.ToDictionaryAsync(season => season.Id, cancellationToken);
            var fixtures = await database.Fixtures.ToDictionaryAsync(fixture => fixture.Id, cancellationToken);
            var statistics = await database.TeamMatchStatistics.ToDictionaryAsync(statistic => $"{statistic.FixtureId}:{statistic.TeamId}", cancellationToken);

            foreach (var item in items)
            {
                var league = item.GetProperty("league");
                var competition = GetCompetition(league);
                var season = GetSeason(league, competition);
                var sides = item.GetProperty("teams");
                var home = GetTeam(sides.GetProperty("home"));
                var away = GetTeam(sides.GetProperty("away"));
                var source = item.GetProperty("fixture");
                var fixtureExternalId = source.GetProperty("id").GetInt32().ToString(CultureInfo.InvariantCulture);
                var score = item.GetProperty("goals");
                var kickoff = DateTimeOffset.Parse(source.GetProperty("date").GetString()!, CultureInfo.InvariantCulture);
                var status = MapStatus(source.GetProperty("status").GetProperty("short").GetString());

                if (mappedIds.TryGetValue($"fixture:{fixtureExternalId}", out var fixtureId) && fixtures.TryGetValue(fixtureId, out var fixture))
                {
                    fixture.ApplyStatus(status, kickoff);
                    fixture.RecordRegulationScore(ToNullableInt(score.GetProperty("home")), ToNullableInt(score.GetProperty("away")));
                    if (status == FixtureStatus.Finished) await SynchronizeStatisticsAsync(fixture, fixtureExternalId, mappedIds, statistics, cancellationToken);
                    continue;
                }

                fixture = new Fixture(Guid.NewGuid(), season.Id, home.Id, away.Id, kickoff, status);
                fixture.RecordRegulationScore(ToNullableInt(score.GetProperty("home")), ToNullableInt(score.GetProperty("away")));
                database.Fixtures.Add(fixture);
                fixtures.Add(fixture.Id, fixture);
                AddMapping("fixture", fixtureExternalId, fixture.Id);
            }

            run.Complete(items.Length, DateTimeOffset.UtcNow);
            await database.SaveChangesAsync(cancellationToken);
            return new ManualFixtureSyncResult(run.Id, items.Length, run.Status);

            Competition GetCompetition(JsonElement source)
            {
                var externalId = source.GetProperty("id").GetInt32().ToString(CultureInfo.InvariantCulture);
                var name = source.GetProperty("name").GetString()!;
                if (mappedIds.TryGetValue($"competition:{externalId}", out var id) && competitions.TryGetValue(id, out var mapped)) return mapped;
                var existing = competitions.Values.FirstOrDefault(competition => competition.Name == name);
                var competition = existing ?? new Competition(Guid.NewGuid(), name);
                if (existing is null) { database.Competitions.Add(competition); competitions.Add(competition.Id, competition); }
                else competition.Rename(name);
                AddMapping("competition", externalId, competition.Id);
                return competition;
            }

            Season GetSeason(JsonElement source, Competition competition)
            {
                var label = source.GetProperty("season").GetInt32().ToString(CultureInfo.InvariantCulture);
                var externalId = $"{source.GetProperty("id").GetInt32().ToString(CultureInfo.InvariantCulture)}:{label}";
                if (mappedIds.TryGetValue($"season:{externalId}", out var id) && seasons.TryGetValue(id, out var mapped)) return mapped;
                var existing = seasons.Values.FirstOrDefault(season => season.CompetitionId == competition.Id && season.Label == label);
                var season = existing ?? new Season(Guid.NewGuid(), competition.Id, label);
                if (existing is null) { database.Seasons.Add(season); seasons.Add(season.Id, season); }
                AddMapping("season", externalId, season.Id);
                return season;
            }

            Team GetTeam(JsonElement source)
            {
                var externalId = source.GetProperty("id").GetInt32().ToString(CultureInfo.InvariantCulture);
                var name = source.GetProperty("name").GetString()!;
                if (mappedIds.TryGetValue($"team:{externalId}", out var id) && teams.TryGetValue(id, out var mapped)) { mapped.Rename(name); return mapped; }
                var existing = teams.Values.FirstOrDefault(team => team.Name == name);
                var team = existing ?? new Team(Guid.NewGuid(), name);
                if (existing is null) { database.Teams.Add(team); teams.Add(team.Id, team); }
                AddMapping("team", externalId, team.Id);
                return team;
            }

            void AddMapping(string resourceType, string externalId, Guid canonicalId)
            {
                var key = $"{resourceType}:{externalId}";
                if (mappedIds.ContainsKey(key)) return;
                database.ProviderEntityMappings.Add(new ProviderEntityMapping(Guid.NewGuid(), "api-football", resourceType, externalId, canonicalId));
                mappedIds.Add(key, canonicalId);
            }
        }
        catch (HttpRequestException)
        {
            run.Fail("provider-request-failed", DateTimeOffset.UtcNow);
            await database.SaveChangesAsync(CancellationToken.None);
            throw;
        }
    }

    private async Task<JsonElement[]> GetAsync(string window, CancellationToken token)
    {
        using var response = await client.GetAsync($"fixtures?team=541&{window}=10", token);
        response.EnsureSuccessStatusCode();
        using var document = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: token) ?? throw new InvalidOperationException("Empty API-Football response.");
        return document.RootElement.GetProperty("response").EnumerateArray().Select(item => item.Clone()).ToArray();
    }

    private async Task SynchronizeStatisticsAsync(Fixture fixture, string externalFixtureId, Dictionary<string, Guid> mappedIds, Dictionary<string, TeamMatchStatistic> statistics, CancellationToken token)
    {
        using var response = await client.GetAsync($"fixtures/statistics?fixture={externalFixtureId}", token);
        if (!response.IsSuccessStatusCode) return;
        using var document = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: token);
        if (document is null) return;
        foreach (var item in document.RootElement.GetProperty("response").EnumerateArray())
        {
            var providerTeamId = item.GetProperty("team").GetProperty("id").GetInt32().ToString(CultureInfo.InvariantCulture);
            if (!mappedIds.TryGetValue($"team:{providerTeamId}", out var teamId) || statistics.ContainsKey($"{fixture.Id}:{teamId}")) continue;
            var values = item.GetProperty("statistics").EnumerateArray().ToDictionary(value => value.GetProperty("type").GetString()!, value => value.GetProperty("value").ToString());
            var statistic = new TeamMatchStatistic(Guid.NewGuid(), fixture.Id, teamId, ParsePercent(values, "Ball Possession"), ParseInt(values, "Total Shots"), ParseInt(values, "Shots on Goal"), ParseInt(values, "Corner Kicks"));
            database.TeamMatchStatistics.Add(statistic);
            statistics.Add($"{fixture.Id}:{teamId}", statistic);
        }
    }

    private static int? ParseInt(Dictionary<string, string> values, string key) => values.TryGetValue(key, out var value) && int.TryParse(value, CultureInfo.InvariantCulture, out var result) ? result : null;
    private static int? ParsePercent(Dictionary<string, string> values, string key) => values.TryGetValue(key, out var value) && int.TryParse(value?.TrimEnd('%'), CultureInfo.InvariantCulture, out var result) ? result : null;

    private static int? ToNullableInt(JsonElement value) => value.ValueKind == JsonValueKind.Null ? null : value.GetInt32();

    private static FixtureStatus MapStatus(string? value) => value switch
    {
        "NS" => FixtureStatus.Scheduled, "HT" => FixtureStatus.HalfTime, "FT" or "AET" or "PEN" => FixtureStatus.Finished,
        "PST" => FixtureStatus.Postponed, "CANC" => FixtureStatus.Cancelled, "1H" or "2H" or "ET" or "BT" or "P" => FixtureStatus.Live,
        _ => FixtureStatus.Unknown
    };
}
