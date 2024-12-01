using Bonyan.Layer.Domain.DomainEvent.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Users.DomainEvents;

/// <summary>
/// Domain event triggered when a user's token is removed.
/// </summary>
public class BonIdentityUserTokenRemovedDomainEvent : IBonDomainEvent
{
    /// <summary>
    /// Gets the user associated with the token.
    /// </summary>
    public BonIdentityUser User { get; }

    /// <summary>
    /// Gets the removed token.
    /// </summary>
    public BonIdentityUserToken Token { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BonIdentityUserTokenRemovedDomainEvent"/> class.
    /// </summary>
    /// <param name="user">The user associated with the removed token.</param>
    /// <param name="token">The removed token.</param>
    public BonIdentityUserTokenRemovedDomainEvent(BonIdentityUser user, BonIdentityUserToken token)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        Token = token ?? throw new ArgumentNullException(nameof(token));
    }
}