using Mis.Application.Football;

namespace Mis.Infrastructure.Persistence;

public sealed class LocalEvidenceStorage(string rootPath) : IEvidenceStorage
{
    private readonly string rootPath = Path.GetFullPath(rootPath);

    public async Task<string> StoreAsync(string contentType, Stream content, CancellationToken cancellationToken)
    {
        var extension = contentType switch { "image/jpeg" => ".jpg", "image/png" => ".png", "image/webp" => ".webp", "video/mp4" => ".mp4", "video/webm" => ".webm", "video/quicktime" => ".mov", _ => throw new ArgumentException("Unsupported evidence content type.", nameof(contentType)) };
        Directory.CreateDirectory(rootPath);
        var objectKey = $"{Guid.NewGuid():N}{extension}";
        var temporaryPath = Path.Combine(rootPath, $"{objectKey}.uploading");
        var finalPath = Path.Combine(rootPath, objectKey);
        try
        {
            await using (var target = File.Create(temporaryPath)) await content.CopyToAsync(target, cancellationToken);
            File.Move(temporaryPath, finalPath);
            return objectKey;
        }
        catch
        {
            if (File.Exists(temporaryPath)) File.Delete(temporaryPath);
            throw;
        }
    }

    public Task DeleteAsync(string objectKey, CancellationToken cancellationToken)
    {
        var path = Path.Combine(rootPath, Path.GetFileName(objectKey));
        if (File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }
}
