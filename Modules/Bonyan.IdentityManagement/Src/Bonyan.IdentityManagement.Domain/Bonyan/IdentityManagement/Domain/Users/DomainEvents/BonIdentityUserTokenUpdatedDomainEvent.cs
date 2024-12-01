using Bonyan.Layer.Domain.DomainEvent.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Users.DomainEvents;

/// <summary>
/// Domain event triggered when a user's token is updated.
/// </summary>
public class BonIdentityUserTokenUpdatedDomainEvent : IBonDomainEvent
{
    /// <summary>
    /// Gets the user associated with the token.
    /// </summary>
    public BonIdentityUser User { get; }

    /// <summary>
    /// Gets the updated token.
    /// </summary>
    public BonIdentityUserToken Token { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BonIdentityUserTokenUpdatedDomainEvent"/> class.
    /// </summary>
    /// <param name="user">The user associated with the updated token.</param>
    /// <param name="token">The updated token.</param>
    public BonIdentityUserTokenUpdatedDomainEvent(BonIdentityUser user, BonIdentityUserToken token)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        Token = token ?? throw new ArgumentNullException(nameof(token));
    }
}