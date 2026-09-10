using Mis.Domain.Football;
using Xunit;

namespace Mis.Domain.Tests;

public sealed class TeamMatchStatisticTests
{
    [Fact]
    public void ConstructorRejectsShotsOnTargetAboveShots()
    {
        Assert.Throws<ArgumentException>(() => new TeamMatchStatistic(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 50, 4, 5, 2));
    }
}
