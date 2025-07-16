using System.Security.Claims;
using Bonyan.IdentityManagement.Permissions;

namespace Bonyan.Novino.Core.Menus
{
    /// <summary>
    /// Represents a menu for a specific location
    /// </summary>
    public class Menu
    {
        /// <summary>
        /// Gets or sets the name of the menu
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the menu location where this menu should be rendered
        /// </summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the menu items
        /// </summary>
        public List<MenuItem> Items { get; set; } = new();

        /// <summary>
        /// Gets or sets the priority of this menu (higher values = higher priority)
        /// </summary>
        public int Priority { get; set; } = 0;

        /// <summary>
        /// Gets or sets whether this menu is enabled
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the metadata for this menu
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();

        /// <summary>
        /// Gets or sets the provider that created this menu
        /// </summary>
        public string? ProviderId { get; set; }

        /// <summary>
        /// Gets or sets the required permissions (authorities) to view this menu
        /// </summary>
        public List<string> Authority { get; set; } = new();

        /// <summary>
        /// Gets or sets whether authentication is required to view this menu
        /// </summary>
        public bool RequiresAuthentication { get; set; } = false;

        /// <summary>
        /// Gets the sorted menu items by order
        /// </summary>
        public IEnumerable<MenuItem> SortedItems => Items.OrderBy(x => x.Order);

        public Menu()
        {
        }

        public Menu(string name, string location)
        {
            Name = name;
            Location = location;
        }

        /// <summary>
        /// Adds a menu item to this menu
        /// </summary>
        /// <param name="item">The menu item to add</param>
        public void AddItem(MenuItem item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// Removes a menu item from this menu
        /// </summary>
        /// <param name="item">The menu item to remove</param>
        public void RemoveItem(MenuItem item)
        {
            Items.Remove(item);
        }

        /// <summary>
        /// Gets menu items visible to the specified user
        /// </summary>
        /// <param name="user">The user to check visibility for</param>
        /// <returns>Filtered menu items</returns>
        public IEnumerable<MenuItem> GetVisibleItems(ClaimsPrincipal? user = null)
        {
            return SortedItems.Where(item => item.CheckIsVisible(user));
        }
    }
    
    /// <summary>
    /// Represents a menu item
    /// </summary>
    public class MenuItem
    {
        /// <summary>
        /// Gets or sets the unique identifier for this menu item
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the display title of the menu item
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL for this menu item
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the icon for this menu item
        /// </summary>
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the order/position of this menu item
        /// </summary>
        public int Order { get; set; } = 0;

        /// <summary>
        /// Gets or sets the CSS classes for this menu item
        /// </summary>
        public string CssClass { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the target for the link (_blank, _self, etc.)
        /// </summary>
        public string Target { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether this menu item is enabled
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets whether this menu item is visible
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Gets or sets the required permissions to view this menu item
        /// </summary>
        public List<string> RequiredPermissions { get; set; } = new();

        /// <summary>
        /// Gets or sets the required roles to view this menu item
        /// </summary>
        public List<string> RequiredRoles { get; set; } = new();

        /// <summary>
        /// Gets or sets the required permissions (authorities) to view this menu item
        /// </summary>
        public List<string> Authority { get; set; } = new();

        /// <summary>
        /// Gets or sets whether authentication is required to view this menu item
        /// </summary>
        public bool RequiresAuthentication { get; set; } = false;

        /// <summary>
        /// Gets or sets custom metadata for this menu item
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();

        /// <summary>
        /// Gets or sets the child menu items
        /// </summary>
        public List<MenuItem> Children { get; set; } = new();

        /// <summary>
        /// Gets or sets the parent menu item
        /// </summary>
        public MenuItem? Parent { get; set; }

        /// <summary>
        /// Gets or sets a custom visibility condition function
        /// </summary>
        public Func<ClaimsPrincipal?, bool>? VisibilityCondition { get; set; }

        /// <summary>
        /// Gets the sorted child menu items by order
        /// </summary>
        public IEnumerable<MenuItem> SortedChildren => Children.OrderBy(x => x.Order);

        /// <summary>
        /// Gets whether this menu item has children
        /// </summary>
        public bool HasChildren => Children.Any();

        /// <summary>
        /// Gets the depth level of this menu item
        /// </summary>
        public int Depth => Parent?.Depth + 1 ?? 0;

        public MenuItem()
        {
        }

        public MenuItem(string title, string url, string icon = "", int order = 0)
        {
            Title = title;
            Url = url;
            Icon = icon;
            Order = order;
        }

        /// <summary>
        /// Adds a child menu item
        /// </summary>
        /// <param name="child">The child menu item to add</param>
        public void AddChild(MenuItem child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        /// <summary>
        /// Removes a child menu item
        /// </summary>
        /// <param name="child">The child menu item to remove</param>
        public void RemoveChild(MenuItem child)
        {
            child.Parent = null;
            Children.Remove(child);
        }

        /// <summary>
        /// Checks if this menu item is visible to the specified user
        /// </summary>
        /// <param name="user">The user to check visibility for</param>
        /// <returns>True if visible, false otherwise</returns>
        public bool CheckIsVisible(ClaimsPrincipal? user = null)
        {
            if (!IsVisible || !IsEnabled)
                return false;

            // Check authentication requirement
            if (RequiresAuthentication && (user == null || !user.Identity?.IsAuthenticated == true))
                return false;

            // Check role requirements
            if (RequiredRoles.Any() && (user == null || !RequiredRoles.Any(role => user.IsInRole(role))))
                return false;

            // Check permission requirements (you can implement your own permission logic here)
            if (RequiredPermissions.Any() && user != null)
            {
                // Example: Check if user has required permissions
                // This assumes you have a permission claim type
                var userPermissions = user.Claims
                    .Where(c => c.Type == BonPermissionClaimTypes.Permission)
                    .Select(c => c.Value)
                    .ToList();

                if (!RequiredPermissions.Any(permission => userPermissions.Contains(permission)))
                    return false;
            }

            // Check authority requirements (permissions)
            if (Authority.Any() && user != null)
            {
                // Check if user has required authorities/permissions
                var userPermissions = user.Claims
                    .Where(c => c.Type == BonPermissionClaimTypes.Permission)
                    .Select(c => c.Value)
                    .ToList();

                if (!Authority.Any(authority => userPermissions.Contains(authority)))
                    return false;
            }

            // Check custom visibility condition
            if (VisibilityCondition != null)
                return VisibilityCondition(user);

            return true;
        }

        /// <summary>
        /// Gets all visible child menu items for the specified user
        /// </summary>
        /// <param name="user">The user to check visibility for</param>
        /// <returns>Filtered child menu items</returns>
        public IEnumerable<MenuItem> GetVisibleChildren(ClaimsPrincipal? user = null)
        {
            return SortedChildren.Where(child => child.CheckIsVisible(user));
        }

        /// <summary>
        /// Gets all descendant menu items (children, grandchildren, etc.)
        /// </summary>
        /// <returns>All descendant menu items</returns>
        public IEnumerable<MenuItem> GetAllDescendants()
        {
            var descendants = new List<MenuItem>();
            
            foreach (var child in Children)
            {
                descendants.Add(child);
                descendants.AddRange(child.GetAllDescendants());
            }
            
            return descendants;
        }

        /// <summary>
        /// Finds a menu item by ID in this item and its descendants
        /// </summary>
        /// <param name="id">The ID to search for</param>
        /// <returns>The menu item if found, null otherwise</returns>
        public MenuItem? FindById(string id)
        {
            if (Id == id)
                return this;

            foreach (var child in Children)
            {
                var found = child.FindById(id);
                if (found != null)
                    return found;
            }

            return null;
        }

        public override string ToString()
        {
            return $"{Title} ({Url})";
        }

        public override bool Equals(object? obj)
        {
            if (obj is MenuItem other)
            {
                return Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode(StringComparison.OrdinalIgnoreCase);
        }
    }
}

