using System.Net.Http.Json;
using System.Text.Json;
using System.Globalization;
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
        var items = (await GetAsync("next", cancellationToken)).Concat(await GetAsync("last", cancellationToken))
            .GroupBy(item => item.GetProperty("fixture").GetProperty("id").GetInt32()).Select(group => group.First()).ToArray();
        var teams = new Dictionary<int, Team>(); var competitions = new Dictionary<int, Competition>(); var seasons = new Dictionary<string, Season>();
        foreach (var item in items)
        {
            var league = item.GetProperty("league"); var leagueId = league.GetProperty("id").GetInt32();
            if (!competitions.TryGetValue(leagueId, out var competition)) { competition = new Competition(Guid.NewGuid(), league.GetProperty("name").GetString()!); competitions.Add(leagueId, competition); database.Competitions.Add(competition); }
            var seasonKey = $"{leagueId}:{league.GetProperty("season").GetInt32().ToString(CultureInfo.InvariantCulture)}";
            if (!seasons.TryGetValue(seasonKey, out var season)) { season = new Season(Guid.NewGuid(), competition.Id, league.GetProperty("season").GetInt32().ToString(CultureInfo.InvariantCulture)); seasons.Add(seasonKey, season); database.Seasons.Add(season); }
            var sides = item.GetProperty("teams"); var home = GetTeam(sides.GetProperty("home"), teams); var away = GetTeam(sides.GetProperty("away"), teams);
            var source = item.GetProperty("fixture"); var score = item.GetProperty("goals");
            var fixture = new Fixture(Guid.NewGuid(), season.Id, home.Id, away.Id, DateTimeOffset.Parse(source.GetProperty("date").GetString()!, CultureInfo.InvariantCulture), MapStatus(source.GetProperty("status").GetProperty("short").GetString()));
            fixture.RecordRegulationScore(ToNullableInt(score.GetProperty("home")), ToNullableInt(score.GetProperty("away")));
            database.Fixtures.Add(fixture);
            database.ProviderEntityMappings.Add(new ProviderEntityMapping(Guid.NewGuid(), "api-football", "fixture", source.GetProperty("id").GetInt32().ToString(CultureInfo.InvariantCulture), fixture.Id));
        }
        run.Complete(items.Length, DateTimeOffset.UtcNow);
        await database.SaveChangesAsync(cancellationToken);
        return new ManualFixtureSyncResult(run.Id, items.Length, run.Status);

        Team GetTeam(JsonElement source, Dictionary<int, Team> known)
        {
            var id = source.GetProperty("id").GetInt32();
            if (known.TryGetValue(id, out var team)) return team;
            team = new Team(Guid.NewGuid(), source.GetProperty("name").GetString()!); known.Add(id, team); database.Teams.Add(team);
            database.ProviderEntityMappings.Add(new ProviderEntityMapping(Guid.NewGuid(), "api-football", "team", id.ToString(CultureInfo.InvariantCulture), team.Id));
            return team;
        }
    }

    private async Task<JsonElement[]> GetAsync(string window, CancellationToken token)
    {
        using var response = await client.GetAsync($"fixtures?team=541&{window}=10", token);
        response.EnsureSuccessStatusCode();
        using var document = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: token) ?? throw new InvalidOperationException("Empty API-Football response.");
        return document.RootElement.GetProperty("response").EnumerateArray().Select(item => item.Clone()).ToArray();
    }

    private static int? ToNullableInt(JsonElement value) => value.ValueKind == JsonValueKind.Null ? null : value.GetInt32();
    private static FixtureStatus MapStatus(string? value) => value switch { "NS" => FixtureStatus.Scheduled, "HT" => FixtureStatus.HalfTime, "FT" or "AET" or "PEN" => FixtureStatus.Finished, "PST" => FixtureStatus.Postponed, "CANC" => FixtureStatus.Cancelled, "1H" or "2H" or "ET" or "BT" or "P" => FixtureStatus.Live, _ => FixtureStatus.Unknown };
}
