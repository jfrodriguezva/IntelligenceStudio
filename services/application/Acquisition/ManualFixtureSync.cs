namespace Mis.Application.Acquisition;

public interface IManualFixtureSync
{
    Task<ManualFixtureSyncResult> ExecuteAsync(CancellationToken cancellationToken);
}

public sealed record ManualFixtureSyncResult(Guid SyncRunId, int ImportedFixtureCount, string Status);
