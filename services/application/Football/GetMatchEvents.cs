namespace Mis.Application.Football;

public sealed record MatchEventSummary(string Type, int Minute, string? Player, string? Note);

public interface IMatchEventQueries
{
    Task<IReadOnlyList<MatchEventSummary>> GetForFixtureAsync(Guid fixtureId, CancellationToken cancellationToken);
}
