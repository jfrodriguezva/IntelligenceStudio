using Mis.Domain.Football;
using Xunit;

namespace Mis.Domain.Tests;

public sealed class TacticalSceneTests
{
    [Fact]
    public void ConstructorAcceptsNormalizedPlayerCoordinates()
    {
        var scene = new TacticalScene(Guid.NewGuid(), Guid.NewGuid(), "Plan de presión", "{\"schemaVersion\":1,\"players\":[{\"playerId\":\"c3c4568d-0fec-4c33-ae4f-9af010c80d4d\",\"role\":\"Forward\",\"x\":0.5,\"y\":0.2}]}", DateTimeOffset.UtcNow);

        Assert.Equal("Plan de presión", scene.Title);
    }

    [Fact]
    public void ConstructorRejectsCoordinatesOutsidePitch()
    {
        Assert.Throws<ArgumentException>(() => new TacticalScene(Guid.NewGuid(), Guid.NewGuid(), "Plan", "{\"schemaVersion\":1,\"players\":[{\"playerId\":\"c3c4568d-0fec-4c33-ae4f-9af010c80d4d\",\"role\":\"Forward\",\"x\":1.1,\"y\":0.2}]}", DateTimeOffset.UtcNow));
    }

    [Fact]
    public void ConstructorRejectsMalformedSceneJson()
    {
        Assert.Throws<ArgumentException>(() => new TacticalScene(Guid.NewGuid(), Guid.NewGuid(), "Plan", "{not-json}", DateTimeOffset.UtcNow));
    }
}
