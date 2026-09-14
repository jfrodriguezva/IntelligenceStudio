namespace Mis.Domain.Football;

public sealed class MatchPrediction
{
    private MatchPrediction() { }
    public MatchPrediction(Guid id, Guid fixtureId, string modelVersion, DateTimeOffset dataCutoff, double homeWin, double draw, double awayWin, double homeStrength, double awayStrength, DateTimeOffset generatedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modelVersion);
        if (homeWin is < 0 or > 1 || draw is < 0 or > 1 || awayWin is < 0 or > 1 || Math.Abs(homeWin + draw + awayWin - 1) > 0.001) throw new ArgumentException("Probabilities must form a distribution.");
        if (homeStrength < 0 || awayStrength < 0) throw new ArgumentOutOfRangeException(nameof(homeStrength));
        Id = id; FixtureId = fixtureId; ModelVersion = modelVersion.Trim(); DataCutoff = dataCutoff; HomeWin = homeWin; Draw = draw; AwayWin = awayWin; HomeStrength = homeStrength; AwayStrength = awayStrength; GeneratedAt = generatedAt;
    }
    public Guid Id { get; private set; } public Guid FixtureId { get; private set; } public string ModelVersion { get; private set; } = string.Empty; public DateTimeOffset DataCutoff { get; private set; } public double HomeWin { get; private set; } public double Draw { get; private set; } public double AwayWin { get; private set; } public double HomeStrength { get; private set; } public double AwayStrength { get; private set; } public DateTimeOffset GeneratedAt { get; private set; }
}
