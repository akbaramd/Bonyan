using Bonyan.Layer.Domain.Aggregate.Abstractions;
using Bonyan.UserManagement.Domain.Enumerations;
using Bonyan.UserManagement.Domain.ValueObjects;

namespace Bonyan.UserManagement.Domain
{
    /// <summary>
    /// Represents a user entity in the domain with properties and methods
    /// for managing a user's profile, contact information, and verification status.
    /// </summary>
    public interface IBonUser : IBonAggregateRoot<BonUserId>
    {
        /// <summary>
        /// Gets or sets the user's unique username.
        /// </summary>
        string UserName { get;  }

        /// <summary>
        /// Gets the user's hashed password.
        /// </summary>
        Password Password { get; }
        
        /// <summary>
        /// Gets or sets the user's email address.
        /// Can be null if the user has not provided an email.
        /// </summary>
        Email? Email { get;  }

        /// <summary>
        /// Gets or sets the user's phone number.
        /// Can be null if the user has not provided a phone number.
        /// </summary>
        PhoneNumber? PhoneNumber { get;  }
        
        /// <summary>
        /// Gets the current status of the user.
        /// </summary>
        UserStatus Status { get; }
        /// <summary>
        /// Updates the user's profile information such as username, email, and phone number.
        /// </summary>
        /// <param name="userName">The new username.</param>
        /// <param name="email">The new email address.</param>
        /// <param name="phoneNumber">The new phone number.</param>
        void UpdateProfile(string userName, Email? email, PhoneNumber? phoneNumber);

    }
}