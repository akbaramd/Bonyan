using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Entities;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users
{
    public class BonIdentityUserRoles<TUser,TRole> : BonEntity
        where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
    {
        public BonUserId UserId { get; private set; }
        public BonRoleId RoleId { get; private set; }

        // Navigation property back to role
        public TRole Role { get; private set; } = default!;

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