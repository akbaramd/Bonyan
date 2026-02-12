using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.Layer.Domain.Entities;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users;

/// <summary>
/// User token entity (part of User aggregate). Value must always be stored as a one-way hash so the raw token is never persisted.
/// </summary>
public class BonIdentityUserToken : BonEntity<BonIdentityUserTokenId>
{
    public BonUserId UserId { get; private set; } = null!;
    public string Type { get; private set; } = string.Empty;
    /// <summary>Hashed token value only; never store or expose the plain token.</summary>
    public string Value { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? Expiration { get; private set; }

    private BonIdentityUserToken() { }

    /// <param name="value">Already-hashed token value. Caller must hash before passing.</param>
    public BonIdentityUserToken(BonUserId userId, string type, string value, DateTime? expiration = null)
    {
        if (userId == null) throw new ArgumentNullException(nameof(userId));
        if (string.IsNullOrWhiteSpace(type)) throw new ArgumentException("Token type cannot be empty.", nameof(type));
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Token value (hashed) cannot be empty.", nameof(value));

        Id = BonIdentityUserTokenId.NewId();
        UserId = userId;
        Type = type;
        Value = value;
        Expiration = expiration;
        CreatedAt = DateTime.UtcNow;
    }

    /// <param name="newValue">Already-hashed token value.</param>
    public void UpdateValue(string newValue, DateTime? newExpiration = null)
    {
        if (string.IsNullOrWhiteSpace(newValue)) throw new ArgumentException("New token value (hashed) cannot be empty.", nameof(newValue));
        Value = newValue;
        UpdatedAt = DateTime.UtcNow;
        if (newExpiration.HasValue) Expiration = newExpiration;
    }

    public bool IsOfType(string type)
    {
        if (string.IsNullOrWhiteSpace(type)) throw new ArgumentException("Token type cannot be empty.", nameof(type));
        return Type.Equals(type, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsExpired() => Expiration.HasValue && DateTime.UtcNow > Expiration.Value;
}
