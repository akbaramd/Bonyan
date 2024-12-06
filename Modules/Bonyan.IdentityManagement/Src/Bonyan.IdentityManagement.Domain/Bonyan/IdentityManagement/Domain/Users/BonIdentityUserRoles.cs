using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Entities;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users
{
    public class BonIdentityUserRoles : BonEntity
    {
        public BonUserId UserId { get; private set; }
        public BonRoleId RoleId { get; private set; }

        // Navigation property back to role
        public BonIdentityRole Role { get; private set; } = default!;

        // Navigation property back to user

        protected BonIdentityUserRoles() { } // For EF Core use

        public BonIdentityUserRoles(BonUserId userId, BonRoleId roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }

        public override object GetKey()
        {
            return new { UserId, RoleId };
        }
    }
}