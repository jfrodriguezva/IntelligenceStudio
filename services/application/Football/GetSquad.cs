namespace Mis.Application.Football;

public sealed record SquadPlayerSummary(Guid Id, string Name, string Position);

public interface ISquadQueries
{
    Task<IReadOnlyList<SquadPlayerSummary>> GetForTeamAsync(Guid teamId, CancellationToken cancellationToken);
}
