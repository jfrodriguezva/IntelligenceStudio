namespace Mis.Application.Football;

public sealed record EvidenceAssetSummary(Guid Id, Guid FixtureId, int Minute, string Description, string OriginalFileName, string ContentType, long ByteLength, DateTimeOffset CapturedAt);
public sealed record CreateEvidenceAssetRequest(int Minute, string Description, string OriginalFileName, string ContentType, long ByteLength, Stream Content);

public interface IEvidenceStorage
{
    Task<string> StoreAsync(string contentType, Stream content, CancellationToken cancellationToken);
    Task DeleteAsync(string objectKey, CancellationToken cancellationToken);
}

public interface IEvidenceAssetService
{
    Task<IReadOnlyList<EvidenceAssetSummary>> GetForFixtureAsync(Guid fixtureId, CancellationToken cancellationToken);
    Task<EvidenceAssetSummary> CreateAsync(Guid fixtureId, CreateEvidenceAssetRequest request, CancellationToken cancellationToken);
}
