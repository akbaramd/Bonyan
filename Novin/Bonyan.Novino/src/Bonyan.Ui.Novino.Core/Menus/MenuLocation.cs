using System.Collections;

namespace Bonyan.Novino.Core.Menus
{
    /// <summary>
    /// Represents a menu location where menus can be rendered
    /// </summary>
    public class MenuLocation 
    {
        /// <summary>
        /// Gets or sets the unique identifier for this menu location
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name for this menu location
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description for this menu location
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether this menu location is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the default menu provider for this location
        /// </summary>
        public Type? DefaultProvider { get; set; }

        /// <summary>
        /// Gets or sets metadata for this menu location
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();

        /// <summary>
        /// Gets or sets the menu provider priority for this location
        /// </summary>
        public int Priority { get; set; } = 0;

        /// <summary>
        /// Gets or sets the maximum depth for menus in this location
        /// </summary>
        public int MaxDepth { get; set; } = 10;

        /// <summary>
        /// Gets or sets whether this location supports nested menus
        /// </summary>
        public bool SupportsNesting { get; set; } = true;

        /// <summary>
        /// Gets or sets the CSS classes for this menu location
        /// </summary>
        public string CssClass { get; set; } = string.Empty;

        public MenuLocation()
        {
        }

        public MenuLocation(string id, string displayName, string description = "")
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
        }

        public override string ToString()
        {
            return $"{DisplayName} ({Id})";
        }

        public override bool Equals(object? obj)
        {
            if (obj is MenuLocation other)
            {
                return Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode(StringComparison.OrdinalIgnoreCase);
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
} 