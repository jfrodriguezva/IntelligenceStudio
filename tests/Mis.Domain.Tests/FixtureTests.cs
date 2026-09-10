using Mis.Domain.Football;
using Xunit;

namespace Mis.Domain.Tests;

public sealed class FixtureTests
{
    [Fact]
    public void RecordRegulationScoreRejectsPartialScore()
    {
        var fixture = CreateFixture();

        Assert.Throws<ArgumentException>(() => fixture.RecordRegulationScore(2, null));
    }

    [Fact]
    public void RecordRegulationScorePersistsKnownScore()
    {
        var fixture = CreateFixture();

        fixture.RecordRegulationScore(3, 1);

        Assert.Equal(3, fixture.RegulationHomeGoals);
        Assert.Equal(1, fixture.RegulationAwayGoals);
    }

    private static Fixture CreateFixture() => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow, FixtureStatus.Scheduled);
}
