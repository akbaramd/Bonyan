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
        public BonIdentityRole Role { get; private set; }

        protected BonIdentityUserRoles() { } // For ORM use

        public BonIdentityUserRoles(BonUserId user, BonRoleId role)
        {
            UserId = user;
            RoleId = role;
        }

        public override object GetKey()
        {
            return new {UserId,RoleId};
        }
    }
}