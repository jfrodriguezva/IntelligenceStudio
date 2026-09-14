namespace Mis.Application.Football;
public sealed record MatchPredictionSummary(Guid Id, Guid FixtureId, string ModelVersion, DateTimeOffset DataCutoff, double HomeWin, double Draw, double AwayWin, DateTimeOffset GeneratedAt);
public interface IMatchPredictionService { Task<MatchPredictionSummary?> GetLatestAsync(Guid fixtureId, CancellationToken cancellationToken); Task<MatchPredictionSummary> GenerateBaselineAsync(Guid fixtureId, CancellationToken cancellationToken); }
public interface IBaselinePredictionRuntime { Task<BaselinePredictionResult> PredictAsync(double homeStrength, double awayStrength, CancellationToken cancellationToken); }
public sealed record BaselinePredictionResult(double HomeWin, double Draw, double AwayWin, string Model);
