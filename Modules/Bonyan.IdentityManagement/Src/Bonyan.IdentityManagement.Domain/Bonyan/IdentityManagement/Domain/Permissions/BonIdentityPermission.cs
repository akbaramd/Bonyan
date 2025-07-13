using System.ComponentModel.DataAnnotations;
using Bonyan.IdentityManagement.Domain.Permissions.DomainEvents;
using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Aggregate;
using Bonyan.Layer.Domain.Entities;

namespace Bonyan.IdentityManagement.Domain.Permissions
{
    public class BonIdentityPermission : BonAggregateRoot<BonPermissionId> 
    {
        // Private constructor to prevent direct instantiation
        private readonly List<BonIdentityRolePermissions> _permissions = new List<BonIdentityRolePermissions>();
        private BonIdentityPermission()
        {
            
        }
        public BonIdentityPermission(BonPermissionId id,string title)
        {
            Id = id;
            Title = title;
            
            AddDomainEvent(new PermissionCreatedEvent(){Name = Title});
        }
        public IReadOnlyCollection<BonIdentityRolePermissions> RolePermissions => _permissions;
        // Static factory method to create instances
        public static BonIdentityPermission Create(BonPermissionId id, string title)
        {
       

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));
            }

            return new BonIdentityPermission(id, title);
        }

        // Title property with validation
        public string Title { get; private set; } = default!;
    }
}