using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Aggregate;
using Bonyan.Layer.Domain.Events;

namespace Bonyan.IdentityManagement.Domain.Roles
{
    public class BonIdentityRole : BonAggregateRoot<BonRoleId>
    {
        // Private constructor for ORM or factory use only.
      

        // Main constructor initializing essential properties
        public BonIdentityRole(BonRoleId id, string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));

            Id = id;
            Title = title;
        }

        // Title property
        public string Title { get; private set; }

        // Method to update title
        public void UpdateTitle(string newTitle)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                throw new ArgumentException("Title cannot be null or empty.", nameof(newTitle));

            Title = newTitle;
        }

     
    }
}
