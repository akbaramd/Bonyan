using Bonyan.Layer.Domain.DomainEvent.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Users.DomainEvents;

/// <summary>
/// Domain event triggered when a user's token is updated.
/// </summary>
public class BonIdentityUserTokenUpdatedDomainEvent : IBonDomainEvent
{
    /// <summary>
    /// Gets the user associated with the updated token.
    /// </summary>
    public readonly object User;

    /// <summary>
    /// Gets the token that was updated.
    /// </summary>
    public readonly object Token;

    /// <summary>
    /// Initializes a new instance of the <see cref="BonIdentityUserTokenUpdatedDomainEvent"/> class.
    /// </summary>
    /// <param name="user">The user associated with the updated token.</param>
    /// <param name="token">The token that was updated.</param>
    public BonIdentityUserTokenUpdatedDomainEvent(object user, object token)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        Token = token ?? throw new ArgumentNullException(nameof(token));
    }
}