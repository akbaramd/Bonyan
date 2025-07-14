using Bonyan.Layer.Domain.DomainEvent.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Users.DomainEvents;

/// <summary>
/// Domain event triggered when a user's token is removed.
/// </summary>
public class BonIdentityUserTokenRemovedDomainEvent : IBonDomainEvent
{
    /// <summary>
    /// Gets the user associated with the removed token.
    /// </summary>
    public readonly object User;

    /// <summary>
    /// Gets the token that was removed.
    /// </summary>
    public readonly object Token;

    /// <summary>
    /// Initializes a new instance of the <see cref="BonIdentityUserTokenRemovedDomainEvent"/> class.
    /// </summary>
    /// <param name="user">The user associated with the removed token.</param>
    /// <param name="token">The token that was removed.</param>
    public BonIdentityUserTokenRemovedDomainEvent(object user, object token)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        Token = token ?? throw new ArgumentNullException(nameof(token));
    }
}