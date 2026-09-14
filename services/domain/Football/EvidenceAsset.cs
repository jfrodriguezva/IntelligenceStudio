namespace Mis.Domain.Football;

public sealed class EvidenceAsset
{
    private EvidenceAsset() { }

    public EvidenceAsset(Guid id, Guid fixtureId, int minute, string description, string originalFileName, string contentType, long byteLength, string objectKey, DateTimeOffset capturedAt)
    {
        if (minute is < 0 or > 130) throw new ArgumentOutOfRangeException(nameof(minute));
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(originalFileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
        ArgumentException.ThrowIfNullOrWhiteSpace(objectKey);
        if (description.Trim().Length > 2000) throw new ArgumentException("Description cannot exceed 2000 characters.", nameof(description));
        if (byteLength is < 1 or > 100_000_000) throw new ArgumentOutOfRangeException(nameof(byteLength));
        Id = id; FixtureId = fixtureId; Minute = minute; Description = description.Trim(); OriginalFileName = originalFileName.Trim(); ContentType = contentType.Trim(); ByteLength = byteLength; ObjectKey = objectKey; CapturedAt = capturedAt;
    }

    public Guid Id { get; private set; }
    public Guid FixtureId { get; private set; }
    public int Minute { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string OriginalFileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long ByteLength { get; private set; }
    public string ObjectKey { get; private set; } = string.Empty;
    public DateTimeOffset CapturedAt { get; private set; }
}
