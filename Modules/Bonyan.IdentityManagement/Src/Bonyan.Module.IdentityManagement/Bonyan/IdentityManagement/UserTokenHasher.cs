using System.Security.Cryptography;
using System.Text;
using Bonyan.IdentityManagement.Domain.Users;

namespace Bonyan.IdentityManagement;

/// <summary>
/// Hashes user token values with SHA256 so they are never stored in plain form.
/// </summary>
public class Sha256UserTokenHasher : IUserTokenHasher
{
    public string Hash(string plainTokenValue)
    {
        if (string.IsNullOrEmpty(plainTokenValue))
            throw new ArgumentException("Token value cannot be null or empty.", nameof(plainTokenValue));
        var bytes = Encoding.UTF8.GetBytes(plainTokenValue);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
