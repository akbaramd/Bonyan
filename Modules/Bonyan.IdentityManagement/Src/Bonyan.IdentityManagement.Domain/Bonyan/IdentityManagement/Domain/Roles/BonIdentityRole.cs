using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Domain.Aggregate;
using Bonyan.Layer.Domain.Events;

namespace Bonyan.IdentityManagement.Domain.Roles
{
    public class BonIdentityRole : BonAggregateRoot<BonRoleId> 
    {
        // Private constructor for ORM or factory use only.
        protected BonIdentityRole()
        {
        }

        
        // Main constructor initializing essential properties
        public BonIdentityRole(BonRoleId id,string title)
        {
            Id = id;
            Title = title;
        }

        // Title and Name properties
        public string Title { get; private set; } = default!;


        // Backing field for permissions, ensuring encapsulation
        private readonly List<BonIdentityPermission> _permissions = new();

        // Public, read-only access to permissions
        public IReadOnlyCollection<BonIdentityPermission> Permissions => _permissions.AsReadOnly();

        // Method to add a permission, ensuring no duplicate entries
        public void AddPermission(BonIdentityPermission identityPermission)
        {
            if (_permissions.Any(p => p.Equals(identityPermission))) return;
            _permissions.Add(identityPermission);
            AddDomainEvent(new PermissionAddedDomainEvent(Id, identityPermission));
        }

        // Method to remove a permission by entity instance
        public void RemovePermission(BonIdentityPermission identityPermission)
        {
            if (_permissions.Remove(identityPermission))
            {
                AddDomainEvent(new PermissionRemovedDomainEvent(Id, identityPermission));
            }
        }

        // Method to clear all permissions
        public void ClearPermissions()
        {
            _permissions.Clear();
            AddDomainEvent(new PermissionsClearedDomainEvent(Id));
        }

        // Additional methods (if any) to enforce business logic can be added here
    }

    // Domain DomainEvents
    public class PermissionAddedDomainEvent(BonRoleId bonRoleId, BonIdentityPermission identityPermission) : BonDomainEventBase
    {
    }

    public class PermissionRemovedDomainEvent(BonRoleId bonRoleId, BonIdentityPermission identityPermission) : BonDomainEventBase
    {
    };

    public class PermissionsClearedDomainEvent(BonRoleId bonRoleId) : BonDomainEventBase
    {
    }
}