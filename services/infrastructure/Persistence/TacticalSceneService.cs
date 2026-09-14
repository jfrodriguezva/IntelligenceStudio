using Microsoft.EntityFrameworkCore;
using Mis.Application.Football;
using Mis.Domain.Football;

namespace Mis.Infrastructure.Persistence;

public sealed class TacticalSceneService(MisDbContext database) : ITacticalSceneService
{
    public async Task<IReadOnlyList<TacticalSceneSummary>> GetForFixtureAsync(Guid fixtureId, CancellationToken cancellationToken) =>
        await database.TacticalScenes.AsNoTracking().Where(scene => scene.FixtureId == fixtureId).OrderByDescending(scene => scene.CreatedAt)
            .Select(scene => new TacticalSceneSummary(scene.Id, scene.FixtureId, scene.Title, scene.StateJson, scene.CreatedAt)).ToListAsync(cancellationToken);

    public async Task<TacticalSceneSummary> CreateAsync(Guid fixtureId, CreateTacticalSceneRequest request, CancellationToken cancellationToken)
    {
        if (!await database.Fixtures.AnyAsync(fixture => fixture.Id == fixtureId, cancellationToken)) throw new KeyNotFoundException();
        var scene = new TacticalScene(Guid.NewGuid(), fixtureId, request.Title, request.StateJson, DateTimeOffset.UtcNow);
        database.TacticalScenes.Add(scene);
        await database.SaveChangesAsync(cancellationToken);
        return new TacticalSceneSummary(scene.Id, scene.FixtureId, scene.Title, scene.StateJson, scene.CreatedAt);
    }
}
