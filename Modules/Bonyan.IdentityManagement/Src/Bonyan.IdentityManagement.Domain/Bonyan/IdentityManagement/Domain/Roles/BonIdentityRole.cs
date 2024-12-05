using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Aggregate;
using System;

namespace Bonyan.IdentityManagement.Domain.Roles
{
   /// <summary>
    /// Represents an identity role in the domain.
    /// Encapsulates role behavior and adheres to DDD principles.
    /// </summary>
    public class BonIdentityRole : BonAggregateRoot<BonRoleId>
    {
        private readonly List<BonIdentityRolePermissions> _permissions = new List<BonIdentityRolePermissions>();

        // Private constructor for ORM or factory use
        protected BonIdentityRole()
        {
        }

        // Main constructor to initialize essential properties
        public BonIdentityRole(BonRoleId id, string title, bool canBeDeleted)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            SetTitle(title);
            CanBeDeleted = canBeDeleted;
        }

        /// <summary>
        /// Gets the title of the role.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Indicates whether the role can be deleted.
        /// </summary>
        public bool CanBeDeleted { get; private set; } = true;

        /// <summary>
        /// Gets the permissions associated with the role.
        /// </summary>
        public IReadOnlyCollection<BonIdentityRolePermissions> RolePermissions => _permissions;

        /// <summary>
        /// Static factory method to create a deletable role.
        /// </summary>
        /// <param name="id">The unique identifier for the role.</param>
        /// <param name="title">The title of the role.</param>
        /// <returns>A new instance of <see cref="BonIdentityRole"/>.</returns>
        public static BonIdentityRole CreateDeletable(BonRoleId id, string title)
        {
            return new BonIdentityRole(id, title, canBeDeleted: true);
        }

        /// <summary>
        /// Static factory method to create a non-deletable role.
        /// </summary>
        /// <param name="id">The unique identifier for the role.</param>
        /// <param name="title">The title of the role.</param>
        /// <returns>A new instance of <see cref="BonIdentityRole"/>.</returns>
        public static BonIdentityRole CreateNonDeletable(BonRoleId id, string title)
        {
            return new BonIdentityRole(id, title, canBeDeleted: false);
        }

        /// <summary>
        /// Updates the title of the role.
        /// </summary>
        /// <param name="newTitle">The new title for the role.</param>
        /// <exception cref="ArgumentException">Thrown if the title is null or empty.</exception>
        public void UpdateTitle(string newTitle)
        {
            SetTitle(newTitle);
        }

        /// <summary>
        /// Assigns a permission to the role.
        /// </summary>
        /// <param name="permissionId">The unique identifier of the permission to assign.</param>
        public void AssignPermission(BonPermissionId permissionId)
        {
            if (_permissions.Any(p => p.PermissionId == permissionId))
                throw new InvalidOperationException($"Permission with ID {permissionId.Value} is already assigned to this role.");

            var rolePermission = new BonIdentityRolePermissions(Id, permissionId);
            _permissions.Add(rolePermission);

            // Optionally, raise a domain event here
            // AddDomainEvent(new RolePermissionAssignedDomainEvent(Id, permissionId));
        }

        /// <summary>
        /// Removes a permission from the role.
        /// </summary>
        /// <param name="permissionId">The unique identifier of the permission to remove.</param>
        public void RemovePermission(BonPermissionId permissionId)
        {
            var permission = _permissions.FirstOrDefault(p => p.PermissionId == permissionId);
            if (permission == null)
                throw new InvalidOperationException($"Permission with ID {permissionId.Value} is not assigned to this role.");

            _permissions.Remove(permission);

            // Optionally, raise a domain event here
            // AddDomainEvent(new RolePermissionRemovedDomainEvent(Id, permissionId));
        }

        /// <summary>
        /// Marks the role as non-deletable.
        /// </summary>
        public void MarkAsNonDeletable()
        {
            CanBeDeleted = false;
        }

        /// <summary>
        /// Marks the role as deletable.
        /// </summary>
        public void MarkAsDeletable()
        {
            CanBeDeleted = true;
        }

        /// <summary>
        /// Deletes the role if it is deletable.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the role is non-deletable.</exception>
        public void Delete()
        {
            if (!CanBeDeleted)
                throw new InvalidOperationException("This role cannot be deleted because it is marked as non-deletable.");

            // Domain event for deletion can be added here if necessary
            // AddDomainEvent(new RoleDeletedDomainEvent(Id));
        }

        /// <summary>
        /// Sets the title of the role with validation.
        /// </summary>
        /// <param name="title">The title to set.</param>
        /// <exception cref="ArgumentException">Thrown if the title is null or empty.</exception>
        private void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));

            Title = title;
        }
    }

}
