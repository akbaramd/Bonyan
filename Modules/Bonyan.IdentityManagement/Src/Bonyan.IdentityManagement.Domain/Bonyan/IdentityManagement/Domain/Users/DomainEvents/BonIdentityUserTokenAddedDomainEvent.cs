using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.Layer.Domain.DomainEvent.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Users.DomainEvents;

/// <summary>
/// Domain event triggered when a user's token is added.
/// </summary>
public class BonIdentityUserTokenAddedDomainEvent : IBonDomainEvent
{
    /// <summary>
    /// Gets the user associated with the new token.
    /// </summary>
    public BonIdentityUser User { get; }

    /// <summary>
    /// Gets the token that was added.
    /// </summary>
    public BonIdentityUserToken Token { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BonIdentityUserTokenAddedDomainEvent"/> class.
    /// </summary>
    /// <param name="user">The user associated with the added token.</param>
    /// <param name="token">The token that was added.</param>
    public BonIdentityUserTokenAddedDomainEvent(BonIdentityUser user, BonIdentityUserToken token)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        Token = token ?? throw new ArgumentNullException(nameof(token));
    }
}