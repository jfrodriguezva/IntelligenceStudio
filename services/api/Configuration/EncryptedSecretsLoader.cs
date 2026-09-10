using System.Security.Cryptography;
using System.Text.Json;

namespace Mis.Api.Configuration;

public static class EncryptedSecretsLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static void AddIfPresent(ConfigurationManager configuration, string contentRootPath)
    {
        var vaultPath = Path.Combine(contentRootPath, "secrets.enc.json");
        if (!File.Exists(vaultPath))
        {
            return;
        }

        var keyText = Environment.GetEnvironmentVariable("MIS_SECRETS_KEY");
        if (string.IsNullOrWhiteSpace(keyText))
        {
            throw new InvalidOperationException("MIS_SECRETS_KEY is required when secrets.enc.json exists.");
        }

        byte[] key;
        try
        {
            key = Convert.FromBase64String(keyText);
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException("MIS_SECRETS_KEY must be base64-encoded.", exception);
        }

        if (key.Length != 32)
        {
            throw new InvalidOperationException("MIS_SECRETS_KEY must decode to exactly 32 bytes.");
        }

        var encrypted = JsonSerializer.Deserialize<EncryptedSecretDocument>(
            File.ReadAllText(vaultPath),
            JsonOptions)
            ?? throw new InvalidOperationException("secrets.enc.json is not a valid encrypted secret document.");

        if (encrypted.Format != 1 || encrypted.Algorithm != "AES-256-GCM")
        {
            throw new InvalidOperationException("secrets.enc.json uses an unsupported format.");
        }

        var nonce = Convert.FromBase64String(encrypted.Nonce);
        var ciphertext = Convert.FromBase64String(encrypted.Ciphertext);
        var tag = Convert.FromBase64String(encrypted.Tag);
        var plaintext = new byte[ciphertext.Length];

        try
        {
            using var cipher = new AesGcm(key, tag.Length);
            cipher.Decrypt(nonce, ciphertext, tag, plaintext);
        }
        catch (CryptographicException exception)
        {
            throw new InvalidOperationException("secrets.enc.json could not be decrypted with MIS_SECRETS_KEY.", exception);
        }

        configuration.AddJsonStream(new MemoryStream(plaintext, writable: false));
    }

    private sealed record EncryptedSecretDocument(
        int Format,
        string Algorithm,
        string Nonce,
        string Ciphertext,
        string Tag);
}
