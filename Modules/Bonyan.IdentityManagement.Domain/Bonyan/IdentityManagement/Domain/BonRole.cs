using Bonyan.Layer.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using Bonyan.Layer.Domain.Aggregates;
using Bonyan.Layer.Domain.Events;

namespace Bonyan.IdentityManagement.Domain
{
    public class BonRole : AggregateRoot<RoleId>, IBonRole
    {
        // Private constructor for ORM or factory use only.
        protected BonRole() { }

        // Main constructor initializing essential properties
        public BonRole(string title, string name)
        {
            Title = title;
            Name = name;
        }

        // Title and Name properties
        public string Title { get; private set; }
        public string Name { get; private set; }

        // Backing field for permissions, ensuring encapsulation
        private readonly List<BonPermission> _permissions = new();
        // Public, read-only access to permissions
        public IReadOnlyCollection<BonPermission> Permissions => _permissions.AsReadOnly();

        // Method to add a permission, ensuring no duplicate entries
        public void AddPermission(BonPermission permission)
        {
            if (_permissions.Any(p => p.Equals(permission))) return;
            _permissions.Add(permission);
            AddDomainEvent(new PermissionAddedEvent(Id, permission));
        }

        // Method to remove a permission by entity instance
        public void RemovePermission(BonPermission permission)
        {
            if (_permissions.Remove(permission))
            {
                AddDomainEvent(new PermissionRemovedEvent(Id, permission));
            }
        }

        // Method to clear all permissions
        public void ClearPermissions()
        {
            _permissions.Clear();
            AddDomainEvent(new PermissionsClearedEvent(Id));
        }

        // Additional methods (if any) to enforce business logic can be added here
    }

    // Domain Events
    public class PermissionAddedEvent(RoleId RoleId, BonPermission Permission) : DomainEventBase {}
    public class PermissionRemovedEvent(RoleId RoleId, BonPermission Permission) : DomainEventBase{};
    public class PermissionsClearedEvent(RoleId RoleId) : DomainEventBase {}
}
