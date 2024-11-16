using Bonyan.Layer.Domain.Aggregate.Abstractions;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.UserManagement.Domain.Users.Entities
{
    /// <summary>
    /// Represents a user entity in the domain with properties and methods
    /// for managing a user's profile, contact information, and verification status.
    /// </summary>
    public interface IBonUser : IBonAggregateRoot<BonUserId>
    {
        string UserName { get; }
        BonUserPassword Password { get; }
        BonUserEmail? Email { get; }
        BonUserPhoneNumber? PhoneNumber { get; }
        UserStatus Status { get; }
        Guid Version { get; }

        void SetPassword(string newPassword);
        void UpdateProfile(string userName, BonUserEmail? email, BonUserPhoneNumber? phoneNumber);
        void SetPhoneNumber(BonUserPhoneNumber bonUserPhoneNumber);
        void SetEmail(BonUserEmail bonUserEmail);
        bool VerifyPassword(string plainPassword);
        void VerifyEmail();
        void VerifyPhoneNumber();
        void ChangeStatus(UserStatus newStatus);
    }
}