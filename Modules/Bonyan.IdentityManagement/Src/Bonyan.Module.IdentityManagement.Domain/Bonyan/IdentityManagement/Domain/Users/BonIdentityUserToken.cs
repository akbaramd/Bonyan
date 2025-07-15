using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.Layer.Domain.Entities;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users;

/// <summary>
/// Represents a token associated with a user in the identity system.
/// </summary>
public class BonIdentityUserToken<TUser,TRole> : BonEntity<BonIdentityUserTokenId>
    where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
{
    /// <summary>
    /// Gets the ID of the user associated with this token.
    /// </summary>
    public BonUserId UserId { get; private set; }

    /// <summary>
    /// Gets the type of the token (e.g., "RefreshToken", "AccessToken").
    /// </summary>
    public string Type { get; private set; }

    /// <summary>
    /// Gets the value of the token.
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// Gets the timestamp when the token was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the timestamp when the token was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Gets the expiration time of the token.
    /// </summary>
    public DateTime? Expiration { get; private set; }

    // Private constructor for EF Core
    private BonIdentityUserToken() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BonIdentityUserToken"/> class.
    /// </summary>
    /// <param name="userId">The ID of the user associated with the token.</param>
    /// <param name="type">The type of the token (e.g., "RefreshToken").</param>
    /// <param name="value">The value of the token.</param>
    /// <param name="expiration">The expiration time of the token (optional).</param>
    public BonIdentityUserToken(BonUserId userId, string type, string value, DateTime? expiration = null)
    {
        if (userId == null) throw new ArgumentNullException(nameof(userId));
        if (string.IsNullOrWhiteSpace(type)) throw new ArgumentException("Token type cannot be empty.", nameof(type));
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Token value cannot be empty.", nameof(value));

        Id = BonIdentityUserTokenId.NewId();
        UserId = userId;
        Type = type;
        Value = value;
        Expiration = expiration;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the value of the token.
    /// </summary>
    /// <param name="newValue">The new value for the token.</param>
    /// <param name="newExpiration">The new expiration time for the token (optional).</param>
    public void UpdateValue(string newValue, DateTime? newExpiration = null)
    {
        if (string.IsNullOrWhiteSpace(newValue))
            throw new ArgumentException("New token value cannot be empty.", nameof(newValue));

        Value = newValue;
        UpdatedAt = DateTime.UtcNow;
        if (newExpiration.HasValue)
        {
            Expiration = newExpiration;
        }
    }

    /// <summary>
    /// Checks if the token is of the specified type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the token is of the specified type; otherwise, false.</returns>
    public bool IsOfType(string type)
    {
        if (string.IsNullOrWhiteSpace(type)) throw new ArgumentException("Token type cannot be empty.", nameof(type));
        return Type.Equals(type, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks whether the token is expired.
    /// </summary>
    /// <returns>True if the token is expired; otherwise, false.</returns>
    public bool IsExpired()
    {
        return Expiration.HasValue && DateTime.UtcNow > Expiration.Value;
    }
}
