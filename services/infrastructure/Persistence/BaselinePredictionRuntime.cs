using System.Net.Http.Json;
using Mis.Application.Football;

namespace Mis.Infrastructure.Persistence;
public sealed class BaselinePredictionRuntime(HttpClient client) : IBaselinePredictionRuntime
{
    public async Task<BaselinePredictionResult> PredictAsync(double homeStrength, double awayStrength, CancellationToken cancellationToken)
    {
        var response = await client.PostAsJsonAsync("/v1/predictions/baseline", new { home_strength = homeStrength, away_strength = awayStrength }, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BaselinePredictionResult>(cancellationToken) ?? throw new InvalidOperationException("ML runtime returned no prediction.");
    }
}
