using Microsoft.EntityFrameworkCore;
using Mis.Application.Football;

namespace Mis.Infrastructure.Persistence;

public sealed class FixtureQueries(MisDbContext database) : IFixtureQueries
{
    public async Task<IReadOnlyList<FixtureSummary>> GetRecentAndUpcomingAsync(CancellationToken cancellationToken) =>
        await (from fixture in database.Fixtures.AsNoTracking()
               join season in database.Seasons.AsNoTracking() on fixture.SeasonId equals season.Id
               join competition in database.Competitions.AsNoTracking() on season.CompetitionId equals competition.Id
               join home in database.Teams.AsNoTracking() on fixture.HomeTeamId equals home.Id
               join away in database.Teams.AsNoTracking() on fixture.AwayTeamId equals away.Id
               orderby fixture.KickoffUtc
               select new FixtureSummary(fixture.Id, competition.Name, season.Label, home.Name, away.Name, fixture.KickoffUtc, fixture.Status.ToString(), fixture.RegulationHomeGoals, fixture.RegulationAwayGoals))
            .Take(20)
            .ToListAsync(cancellationToken);

    public Task<FixtureSummary?> GetByIdAsync(Guid fixtureId, CancellationToken cancellationToken) =>
        (from fixture in database.Fixtures.AsNoTracking()
         join season in database.Seasons.AsNoTracking() on fixture.SeasonId equals season.Id
         join competition in database.Competitions.AsNoTracking() on season.CompetitionId equals competition.Id
         join home in database.Teams.AsNoTracking() on fixture.HomeTeamId equals home.Id
         join away in database.Teams.AsNoTracking() on fixture.AwayTeamId equals away.Id
         where fixture.Id == fixtureId
         select new FixtureSummary(fixture.Id, competition.Name, season.Label, home.Name, away.Name, fixture.KickoffUtc, fixture.Status.ToString(), fixture.RegulationHomeGoals, fixture.RegulationAwayGoals))
        .SingleOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<TeamMatchStatisticSummary>> GetStatisticsAsync(Guid fixtureId, CancellationToken cancellationToken) =>
        await (from statistic in database.TeamMatchStatistics.AsNoTracking()
               join team in database.Teams.AsNoTracking() on statistic.TeamId equals team.Id
               where statistic.FixtureId == fixtureId
               select new TeamMatchStatisticSummary(team.Name, statistic.PossessionPercent, statistic.Shots, statistic.ShotsOnTarget, statistic.Corners))
            .ToListAsync(cancellationToken);
}
