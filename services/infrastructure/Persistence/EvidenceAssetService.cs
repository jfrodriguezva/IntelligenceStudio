using Microsoft.EntityFrameworkCore;
using Mis.Application.Football;
using Mis.Domain.Football;

namespace Mis.Infrastructure.Persistence;

public sealed class EvidenceAssetService(MisDbContext database, IEvidenceStorage storage) : IEvidenceAssetService
{
    public async Task<IReadOnlyList<EvidenceAssetSummary>> GetForFixtureAsync(Guid fixtureId, CancellationToken cancellationToken) =>
        await database.EvidenceAssets.AsNoTracking().Where(asset => asset.FixtureId == fixtureId).OrderBy(asset => asset.Minute).ThenBy(asset => asset.CapturedAt)
            .Select(asset => new EvidenceAssetSummary(asset.Id, asset.FixtureId, asset.Minute, asset.Description, asset.OriginalFileName, asset.ContentType, asset.ByteLength, asset.CapturedAt)).ToListAsync(cancellationToken);

    public async Task<EvidenceAssetSummary> CreateAsync(Guid fixtureId, CreateEvidenceAssetRequest request, CancellationToken cancellationToken)
    {
        if (!await database.Fixtures.AnyAsync(fixture => fixture.Id == fixtureId, cancellationToken)) throw new KeyNotFoundException();
        var objectKey = await storage.StoreAsync(request.ContentType, request.Content, cancellationToken);
        try
        {
            var asset = new EvidenceAsset(Guid.NewGuid(), fixtureId, request.Minute, request.Description, request.OriginalFileName, request.ContentType, request.ByteLength, objectKey, DateTimeOffset.UtcNow);
            database.EvidenceAssets.Add(asset);
            await database.SaveChangesAsync(cancellationToken);
            return new EvidenceAssetSummary(asset.Id, asset.FixtureId, asset.Minute, asset.Description, asset.OriginalFileName, asset.ContentType, asset.ByteLength, asset.CapturedAt);
        }
        catch
        {
            await storage.DeleteAsync(objectKey, CancellationToken.None);
            throw;
        }
    }
}
