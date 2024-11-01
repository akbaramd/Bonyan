using Bonyan.Layer.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using Bonyan.Layer.Domain.Aggregates;
using Bonyan.Layer.Domain.Events;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.ValueObjects;

namespace Bonyan.IdentityManagement.Domain
{
    public class BonUserRole<TUser,TRole> : Entity where TUser : BonyanUser where TRole : BonRole
    {
        // Private constructor for ORM or factory use only.
        protected BonUserRole() { }

        public BonUserRole(UserId userId, RoleId roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }

        public TUser User { get; set; }
        public UserId UserId { get; set; }
        
        public TRole Role { get; set; }
        public RoleId RoleId { get; set; }

        public override object[] GetKeys()
        {
            return [UserId, RoleId];
        }
    }

}
