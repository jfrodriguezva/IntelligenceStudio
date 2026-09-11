using Microsoft.EntityFrameworkCore;
using Mis.Application.Football;

namespace Mis.Infrastructure.Persistence;

public sealed class FixtureQueries(MisDbContext database) : IFixtureQueries
{
    public async Task<IReadOnlyList<FixtureSummary>> GetRecentAndUpcomingAsync(CancellationToken cancellationToken) =>
        (await SearchAsync(new FixtureSearch(null, null, null, null, null, 1, 20), cancellationToken)).Items;

    public async Task<FixturePage> SearchAsync(FixtureSearch search, CancellationToken cancellationToken)
    {
        var page = Math.Max(search.Page, 1);
        var pageSize = Math.Clamp(search.PageSize, 1, 50);
        var query = from fixture in database.Fixtures.AsNoTracking()
                    join season in database.Seasons.AsNoTracking() on fixture.SeasonId equals season.Id
                    join competition in database.Competitions.AsNoTracking() on season.CompetitionId equals competition.Id
                    join home in database.Teams.AsNoTracking() on fixture.HomeTeamId equals home.Id
                    join away in database.Teams.AsNoTracking() on fixture.AwayTeamId equals away.Id
                    select new { fixture, season, competition, home, away };

        if (!string.IsNullOrWhiteSpace(search.Competition)) query = query.Where(item => item.competition.Name == search.Competition);
        if (!string.IsNullOrWhiteSpace(search.Season)) query = query.Where(item => item.season.Label == search.Season);
        if (!string.IsNullOrWhiteSpace(search.Status)) query = query.Where(item => item.fixture.Status.ToString() == search.Status);
        if (search.From is not null) query = query.Where(item => item.fixture.KickoffUtc >= new DateTimeOffset(search.From.Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero));
        if (search.To is not null) query = query.Where(item => item.fixture.KickoffUtc < new DateTimeOffset(search.To.Value.AddDays(1).ToDateTime(TimeOnly.MinValue), TimeSpan.Zero));

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(item => item.fixture.KickoffUtc).ThenBy(item => item.fixture.Id)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(item => new FixtureSummary(item.fixture.Id, item.competition.Name, item.season.Label, item.fixture.HomeTeamId, item.home.Name, item.fixture.AwayTeamId, item.away.Name, item.fixture.KickoffUtc, item.fixture.Status.ToString(), item.fixture.RegulationHomeGoals, item.fixture.RegulationAwayGoals))
            .ToListAsync(cancellationToken);
        return new FixturePage(items, totalCount, page, pageSize);
    }

    public Task<FixtureSummary?> GetByIdAsync(Guid fixtureId, CancellationToken cancellationToken) =>
        (from fixture in database.Fixtures.AsNoTracking()
         join season in database.Seasons.AsNoTracking() on fixture.SeasonId equals season.Id
         join competition in database.Competitions.AsNoTracking() on season.CompetitionId equals competition.Id
         join home in database.Teams.AsNoTracking() on fixture.HomeTeamId equals home.Id
         join away in database.Teams.AsNoTracking() on fixture.AwayTeamId equals away.Id
         where fixture.Id == fixtureId
         select new FixtureSummary(fixture.Id, competition.Name, season.Label, fixture.HomeTeamId, home.Name, fixture.AwayTeamId, away.Name, fixture.KickoffUtc, fixture.Status.ToString(), fixture.RegulationHomeGoals, fixture.RegulationAwayGoals))
        .SingleOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<TeamMatchStatisticSummary>> GetStatisticsAsync(Guid fixtureId, CancellationToken cancellationToken) =>
        await (from statistic in database.TeamMatchStatistics.AsNoTracking()
               join team in database.Teams.AsNoTracking() on statistic.TeamId equals team.Id
               where statistic.FixtureId == fixtureId
               select new TeamMatchStatisticSummary(team.Name, statistic.PossessionPercent, statistic.Shots, statistic.ShotsOnTarget, statistic.Corners))
            .ToListAsync(cancellationToken);
}
