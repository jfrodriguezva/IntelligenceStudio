using System.Security.Cryptography;
using System.Text;

namespace Mis.Api.Configuration;

public static class AdminRefreshAuthorization
{
    public const string HeaderName = "X-MIS-Admin-Key";

    public static bool IsAuthorized(string? suppliedKey, string? configuredKey)
    {
        if (string.IsNullOrWhiteSpace(suppliedKey) || string.IsNullOrWhiteSpace(configuredKey)) return false;
        return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(suppliedKey), Encoding.UTF8.GetBytes(configuredKey));
    }
}
