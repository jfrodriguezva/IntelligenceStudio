using Microsoft.EntityFrameworkCore;
using Mis.Application.Football;
using Mis.Domain.Football;

namespace Mis.Infrastructure.Persistence;

public sealed class TeamFormQueries(MisDbContext database) : ITeamFormQueries
{
    public async Task<IReadOnlyList<TeamFormSummary>> GetForFixtureAsync(Guid fixtureId, CancellationToken cancellationToken)
    {
        var fixture = await database.Fixtures.AsNoTracking().SingleOrDefaultAsync(item => item.Id == fixtureId, cancellationToken);
        if (fixture is null) return [];
        return [await GetAsync(fixture.HomeTeamId), await GetAsync(fixture.AwayTeamId)];

        async Task<TeamFormSummary> GetAsync(Guid teamId)
        {
            var name = await database.Teams.Where(team => team.Id == teamId).Select(team => team.Name).SingleAsync(cancellationToken);
            var fixtures = await database.Fixtures.AsNoTracking().Where(item => (item.HomeTeamId == teamId || item.AwayTeamId == teamId) && item.Status == FixtureStatus.Finished && item.RegulationHomeGoals != null).OrderByDescending(item => item.KickoffUtc).Take(5).ToListAsync(cancellationToken);
            var results = fixtures.Select(item => new { For = item.HomeTeamId == teamId ? item.RegulationHomeGoals!.Value : item.RegulationAwayGoals!.Value, Against = item.HomeTeamId == teamId ? item.RegulationAwayGoals!.Value : item.RegulationHomeGoals!.Value }).ToArray();
            return new TeamFormSummary(name, results.Length, results.Count(result => result.For > result.Against), results.Count(result => result.For == result.Against), results.Count(result => result.For < result.Against), results.Sum(result => result.For), results.Sum(result => result.Against), results.Sum(result => result.For > result.Against ? 3 : result.For == result.Against ? 1 : 0));
        }
    }
}
