using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.UserManagement.Domain.Users.Entities;
using Bonyan.UserManagement.Domain.Users.Enumerations;

namespace Bonyan.UserManagement.Domain.Users.DomainEvents
{
    /// <summary>
    /// Domain event that is raised when a user's status changes.
    /// </summary>
    public class BonUserStatusChangedDomainEvent : IBonDomainEvent
    {
        /// <summary>
        /// Gets the user whose status changed.
        /// </summary>
        public IBonUser User { get; }

        /// <summary>
        /// Gets the new status assigned to the user.
        /// </summary>
        public UserStatus NewStatus { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BonUserStatusChangedDomainEvent"/> class.
        /// </summary>
        /// <param name="user">The user whose status changed.</param>
        /// <param name="newStatus">The new status assigned to the user.</param>
        public BonUserStatusChangedDomainEvent(IBonUser user, UserStatus newStatus)
        {
            User = user;
            NewStatus = newStatus;
        }
    }
}