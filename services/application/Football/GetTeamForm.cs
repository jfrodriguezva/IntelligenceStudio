namespace Mis.Application.Football;

public sealed record TeamFormSummary(string Team, int Played, int Won, int Drawn, int Lost, int GoalsFor, int GoalsAgainst, int Points);

public interface ITeamFormQueries
{
    Task<IReadOnlyList<TeamFormSummary>> GetForFixtureAsync(Guid fixtureId, CancellationToken cancellationToken);
}
