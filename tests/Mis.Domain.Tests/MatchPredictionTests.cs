using Mis.Domain.Football;
using Xunit;

namespace Mis.Domain.Tests;

public sealed class MatchPredictionTests
{
    [Fact]
    public void ConstructorAcceptsACompleteProbabilityDistribution()
    {
        var prediction = new MatchPrediction(Guid.NewGuid(), Guid.NewGuid(), "baseline-v1", DateTimeOffset.UtcNow, .5, .25, .25, 1.2, 1.1, DateTimeOffset.UtcNow);
        Assert.Equal("baseline-v1", prediction.ModelVersion);
    }

    [Fact]
    public void ConstructorRejectsAnIncompleteProbabilityDistribution()
    {
        Assert.Throws<ArgumentException>(() => new MatchPrediction(Guid.NewGuid(), Guid.NewGuid(), "baseline-v1", DateTimeOffset.UtcNow, .5, .25, .1, 1.2, 1.1, DateTimeOffset.UtcNow));
    }
}
