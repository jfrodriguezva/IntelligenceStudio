using Mis.Domain.Football;
using Xunit;

namespace Mis.Domain.Tests;

public sealed class EvidenceAssetTests
{
    [Fact]
    public void ConstructorRejectsOversizedAsset()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new EvidenceAsset(Guid.NewGuid(), Guid.NewGuid(), 10, "Pressing clip", "clip.mp4", "video/mp4", 100_000_001, "asset.mp4", DateTimeOffset.UtcNow));
    }

    [Fact]
    public void ConstructorTrimsDescription()
    {
        var asset = new EvidenceAsset(Guid.NewGuid(), Guid.NewGuid(), 10, " Pressing clip ", "clip.mp4", "video/mp4", 10, "asset.mp4", DateTimeOffset.UtcNow);
        Assert.Equal("Pressing clip", asset.Description);
    }
}
