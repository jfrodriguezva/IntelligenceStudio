namespace Mis.Application.Acquisition;

public interface ISyncRunQueries
{
    Task<IReadOnlyList<SyncRunSummary>> GetRecentAsync(int take, CancellationToken cancellationToken);
}

public sealed record SyncRunSummary(
    Guid Id,
    string Provider,
    string Status,
    DateTimeOffset RequestedAt,
    DateTimeOffset? CompletedAt,
    int ImportedFixtureCount,
    string? ErrorCode);
