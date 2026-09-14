namespace Mis.Application.Football;

public sealed record TacticalSceneSummary(Guid Id, Guid FixtureId, string Title, string StateJson, DateTimeOffset CreatedAt);
public sealed record CreateTacticalSceneRequest(string Title, string StateJson);

public interface ITacticalSceneService
{
    Task<IReadOnlyList<TacticalSceneSummary>> GetForFixtureAsync(Guid fixtureId, CancellationToken cancellationToken);
    Task<TacticalSceneSummary> CreateAsync(Guid fixtureId, CreateTacticalSceneRequest request, CancellationToken cancellationToken);
}
