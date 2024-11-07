using Bonyan.Layer.Domain.Enumerations;

namespace Bonyan.UserManagement.Domain.Enumerations
{
    /// <summary>
    /// Represents the status of a user within the system, allowing various states for user accounts.
    /// </summary>
    public class UserStatus : BonEnumeration
    {
        public static readonly UserStatus Active = new(IdActive, nameof(Active));
        public static readonly UserStatus PendingVerification = new(IdPendingVerification, nameof(PendingVerification));
        public static readonly UserStatus Suspended = new(IdSuspended, nameof(Suspended));
        public static readonly UserStatus Inactive = new(IdInactive, nameof(Inactive));
        public static readonly UserStatus Deactivated = new(IdDeactivated, nameof(Deactivated));
        public static readonly UserStatus Banned = new(IdBanned, nameof(Banned));
        public static readonly UserStatus PendingDeletion = new(IdPendingDeletion, nameof(PendingDeletion));
        public static readonly UserStatus PendingApproval = new(IdPendingApproval, nameof(PendingApproval));
        public static readonly UserStatus Archived = new(IdArchived, nameof(Archived));

        // ID Constants
        public const int IdActive = 1;
        public const int IdPendingVerification = 2;
        public const int IdSuspended = 3;
        public const int IdInactive = 4;
        public const int IdDeactivated = 5;
        public const int IdBanned = 6;
        public const int IdPendingDeletion = 7;
        public const int IdPendingApproval = 8;
        public const int IdArchived = 9;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStatus"/> class.
        /// </summary>
        private UserStatus(int id, string name) : base(id, name) { }
    }
}