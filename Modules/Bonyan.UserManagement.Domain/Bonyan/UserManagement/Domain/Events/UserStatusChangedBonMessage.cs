using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.UserManagement.Domain.Enumerations;

namespace Bonyan.UserManagement.Domain.Events
{
    /// <summary>
    /// Domain event that is raised when a user's status changes.
    /// </summary>
    public class UserStatusChangedDomainEvent : IBonDomainEvent
    {
        /// <summary>
        /// Gets the user whose status changed.
        /// </summary>
        public BonUser User { get; }

        /// <summary>
        /// Gets the new status assigned to the user.
        /// </summary>
        public UserStatus NewStatus { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStatusChangedDomainEvent"/> class.
        /// </summary>
        /// <param name="user">The user whose status changed.</param>
        /// <param name="newStatus">The new status assigned to the user.</param>
        public UserStatusChangedDomainEvent(BonUser user, UserStatus newStatus)
        {
            User = user;
            NewStatus = newStatus;
        }
    }
}