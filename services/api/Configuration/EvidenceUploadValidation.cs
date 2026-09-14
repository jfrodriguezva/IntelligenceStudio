using Microsoft.AspNetCore.Http;

namespace Mis.Api.Configuration;

public static class EvidenceUploadValidation
{
    private static readonly HashSet<string> AllowedTypes = ["image/jpeg", "image/png", "image/webp", "video/mp4", "video/webm", "video/quicktime"];

    public static bool IsAllowed(IFormFile? file)
    {
        if (file is null || file.Length is < 1 or > 100_000_000 || !AllowedTypes.Contains(file.ContentType)) return false;
        using var content = file.OpenReadStream();
        Span<byte> header = stackalloc byte[12];
        var read = content.Read(header);
        return file.ContentType switch
        {
            "image/jpeg" => read >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            "image/png" => read >= 8 && header[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            "image/webp" => read >= 12 && header[..4].SequenceEqual("RIFF"u8) && header[8..12].SequenceEqual("WEBP"u8),
            "video/mp4" or "video/quicktime" => read >= 8 && header[4..8].SequenceEqual("ftyp"u8),
            "video/webm" => read >= 4 && header[..4].SequenceEqual(new byte[] { 0x1A, 0x45, 0xDF, 0xA3 }),
            _ => false,
        };
    }
}
