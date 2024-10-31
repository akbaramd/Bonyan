using System.Collections.Generic;
using Bonyan.Layer.Domain.Aggregates;
using Bonyan.Layer.Domain.ValueObjects;
using Bonyan.Layer.Domain.Events;
using Bonyan.UserManagement.Domain.Enumerations;
using Bonyan.UserManagement.Domain.Events;
using Bonyan.UserManagement.Domain.ValueObjects;

namespace Bonyan.UserManagement.Domain
{
    /// <summary>
    /// Represents a user entity in the domain with properties and methods
    /// for managing a user's profile, contact information, verification status, and overall status.
    /// </summary>
    public class BonyanUser : FullAuditableAggregateRoot<UserId>, IBonyanUser
    {
        /// <summary>
        /// Gets or sets the user's unique username.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets the user's hashed password.
        /// </summary>
        public Password Password { get; private set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// Can be null if the user has not provided an email.
        /// </summary>
        public Email? Email { get; private set; }

        /// <summary>
        /// Gets or sets the user's phone number.
        /// Can be null if the user has not provided a phone number.
        /// </summary>
        public PhoneNumber? PhoneNumber { get; private set; }

        /// <summary>
        /// Gets the current status of the user.
        /// </summary>
        public UserStatus Status { get; private set; }

        protected BonyanUser(){}
        
        /// <summary>
        /// Initializes a new user with basic information and sets the default status to Active.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <param name="userName">The unique username of the user.</param>
        /// <param name="initialPassword">The initial password of the user.</param>
        public BonyanUser(UserId id, string userName, string initialPassword)
        {
            Id = id;
            UserName = userName;
            Password = new Password(initialPassword);
            Status = UserStatus.Active; // Default status
            AddDomainEvent(new UserCreatedEvent(this));
        }

        /// <summary>
        /// Updates the user's profile information such as username, email, and phone number.
        /// </summary>
        /// <param name="userName">The new username.</param>
        /// <param name="email">The new email address.</param>
        /// <param name="phoneNumber">The new phone number.</param>
        public void UpdateProfile(string userName, Email? email, PhoneNumber? phoneNumber)
        {
            UserName = userName;
            Email = email;
            PhoneNumber = phoneNumber;
            AddDomainEvent(new UserProfileUpdatedEvent(this));
        }

        /// <summary>
        /// Sets a new password for the user.
        /// </summary>
        /// <param name="newPassword">The new password in plain text.</param>
        public void SetPassword(string newPassword)
        {
            Password = new Password(newPassword);
            AddDomainEvent(new PasswordChangedEvent(this));
        }

        /// <summary>
        /// Verifies if the provided plain password matches the user's stored hashed password.
        /// </summary>
        /// <param name="plainPassword">The password to verify in plain text.</param>
        /// <returns>True if the password matches; otherwise, false.</returns>
        public bool VerifyPassword(string plainPassword)
        {
            return Password.Verify(plainPassword);
        }

        /// <summary>
        /// Marks the user's email as verified.
        /// </summary>
        public void VerifyEmail()
        {
            if (Email != null && !Email.IsVerified)
            {
                Email.Verify();
                AddDomainEvent(new EmailVerifiedEvent(this));
            }
        }

        /// <summary>
        /// Marks the user's phone number as verified.
        /// </summary>
        public void VerifyPhoneNumber()
        {
            if (PhoneNumber != null && !PhoneNumber.IsVerified)
            {
                PhoneNumber.Verify();
                AddDomainEvent(new PhoneNumberVerifiedEvent(this));
            }
        }

        /// <summary>
        /// Changes the status of the user to a new status and triggers a domain event if the status changes.
        /// </summary>
        /// <param name="newStatus">The new status to set for the user.</param>
        public void ChangeStatus(UserStatus newStatus)
        {
            if (Status != newStatus)
            {
                Status = newStatus;
                AddDomainEvent(new UserStatusChangedEvent(this, newStatus));
            }
        }
    }
}
