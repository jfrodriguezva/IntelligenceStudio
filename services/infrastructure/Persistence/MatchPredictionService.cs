using Microsoft.EntityFrameworkCore;
using Mis.Application.Football;
using Mis.Domain.Football;

namespace Mis.Infrastructure.Persistence;
public sealed class MatchPredictionService(MisDbContext database, IBaselinePredictionRuntime runtime) : IMatchPredictionService
{
    public async Task<MatchPredictionSummary?> GetLatestAsync(Guid fixtureId, CancellationToken cancellationToken) => await database.MatchPredictions.AsNoTracking().Where(x => x.FixtureId == fixtureId).OrderByDescending(x => x.GeneratedAt).Select(x => new MatchPredictionSummary(x.Id,x.FixtureId,x.ModelVersion,x.DataCutoff,x.HomeWin,x.Draw,x.AwayWin,x.GeneratedAt)).FirstOrDefaultAsync(cancellationToken);
    public async Task<MatchPredictionSummary> GenerateBaselineAsync(Guid fixtureId, CancellationToken cancellationToken)
    {
        var fixture = await database.Fixtures.SingleOrDefaultAsync(x => x.Id == fixtureId, cancellationToken) ?? throw new KeyNotFoundException();
        var cutoff = fixture.KickoffUtc ?? DateTimeOffset.UtcNow;
        async Task<double> Strength(Guid teamId) { var previous = await database.Fixtures.AsNoTracking().Where(x => (x.HomeTeamId == teamId || x.AwayTeamId == teamId) && x.Status == FixtureStatus.Finished && x.RegulationHomeGoals != null && x.KickoffUtc < cutoff).OrderByDescending(x => x.KickoffUtc).Take(5).ToListAsync(cancellationToken); var points = previous.Sum(x => (x.HomeTeamId == teamId ? x.RegulationHomeGoals!.Value > x.RegulationAwayGoals!.Value ? 3 : x.RegulationHomeGoals == x.RegulationAwayGoals ? 1 : 0 : x.RegulationAwayGoals!.Value > x.RegulationHomeGoals!.Value ? 3 : x.RegulationAwayGoals == x.RegulationHomeGoals ? 1 : 0)); return 1 + (double)points / Math.Max(previous.Count * 3, 1); }
        var home = await Strength(fixture.HomeTeamId); var away = await Strength(fixture.AwayTeamId); var result = await runtime.PredictAsync(home, away, cancellationToken);
        var prediction = new MatchPrediction(Guid.NewGuid(), fixtureId, result.Model, cutoff, result.HomeWin, result.Draw, result.AwayWin, home, away, DateTimeOffset.UtcNow); database.MatchPredictions.Add(prediction); await database.SaveChangesAsync(cancellationToken);
        return new MatchPredictionSummary(prediction.Id,prediction.FixtureId,prediction.ModelVersion,prediction.DataCutoff,prediction.HomeWin,prediction.Draw,prediction.AwayWin,prediction.GeneratedAt);
    }
}
