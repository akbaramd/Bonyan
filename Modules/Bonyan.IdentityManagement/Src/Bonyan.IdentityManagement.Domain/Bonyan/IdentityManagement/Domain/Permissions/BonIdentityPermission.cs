using System.ComponentModel.DataAnnotations;
using Bonyan.Layer.Domain.Entities;

namespace Bonyan.IdentityManagement.Domain.Permissions
{
    public class BonIdentityPermission : BonEntity
    {
        // Private constructor to prevent direct instantiation

        private BonIdentityPermission()
        {
            
        }
        public BonIdentityPermission(string key, string title)
        {
            Key = key;
            Title = title;
        }

        // Static factory method to create instances
        public static BonIdentityPermission Create(string key, string title)
        {
            // Validate the inputs
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));
            }

            return new BonIdentityPermission(key, title);
        }

        // Overrides the GetKey method to return the Key property
        public override object GetKey()
        {
            return Key;
        }

        // Key property with validation
        [Key]
        public string Key { get; private set; } = default!;

        // Title property with validation
        public string Title { get; private set; } = default!;
    }
}