using Bonyan.Layer.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using Bonyan.Layer.Domain.Aggregates;
using Bonyan.Layer.Domain.Events;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.ValueObjects;

namespace Bonyan.IdentityManagement.Domain
{
    public class BonUserRole<TUser,TRole> : BonEntity where TUser : BonUser where TRole : BonRole
    {
        // Private constructor for ORM or factory use only.
        protected BonUserRole() { }

        public BonUserRole(BonUserId bonUserId, BonRoleId bonRoleId)
        {
            BonUserId = bonUserId;
            BonRoleId = bonRoleId;
        }

        public TUser User { get; set; }
        public BonUserId BonUserId { get; set; }
        
        public TRole Role { get; set; }
        public BonRoleId BonRoleId { get; set; }

        public override object[] GetKeys()
        {
            return [BonUserId, BonRoleId];
        }
    }

}
