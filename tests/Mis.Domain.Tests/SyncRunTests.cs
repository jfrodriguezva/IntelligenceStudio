using Mis.Domain.Acquisition;
using Xunit;

namespace Mis.Domain.Tests;

public sealed class SyncRunTests
{
    [Fact]
    public void FailRecordsTraceableFailure()
    {
        var run = new SyncRun(Guid.NewGuid(), "api-football", DateTimeOffset.UtcNow);

        run.Fail("provider-request-failed", DateTimeOffset.UtcNow);

        Assert.Equal("Failed", run.Status);
        Assert.Equal("provider-request-failed", run.ErrorCode);
        Assert.NotNull(run.CompletedAt);
    }
}
