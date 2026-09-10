namespace Mis.Domain.Acquisition;

public sealed class SyncRun
{
    private SyncRun() { }

    public SyncRun(Guid id, string provider, DateTimeOffset requestedAt)
    {
        Id = id;
        Provider = provider;
        RequestedAt = requestedAt;
        Status = "Running";
    }

    public Guid Id { get; private set; }
    public string Provider { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public DateTimeOffset RequestedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public int ImportedFixtureCount { get; private set; }
    public string? ErrorCode { get; private set; }

    public void Complete(int importedFixtureCount, DateTimeOffset completedAt)
    {
        ImportedFixtureCount = importedFixtureCount;
        CompletedAt = completedAt;
        Status = "Succeeded";
    }
}
