namespace Bonyan.IdentityManagement.Permissions;

/// <summary>
/// Represents a group of related permissions
/// </summary>
public class PermissionGroupDefinition
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<PermissionDefinition> Permissions { get; set; } = new();
    public Dictionary<string, object> Properties { get; set; } = new();

    public PermissionGroupDefinition()
    {
    }

    public PermissionGroupDefinition(string name, string? displayName = null, string? description = null)
    {
        Name = name;
        DisplayName = displayName ?? name;
        Description = description;
    }

    /// <summary>
    /// Adds a permission to this group
    /// </summary>
    public PermissionDefinition AddPermission(string name, string? displayName = null, string? description = null)
    {
        var permission = new PermissionDefinition(name, displayName, description)
        {
            GroupName = Name
        };
        
        Permissions.Add(permission);
        return permission;
    }

    /// <summary>
    /// Gets all permissions in this group (including children)
    /// </summary>
    public IEnumerable<PermissionDefinition> GetAllPermissions()
    {
        foreach (var permission in Permissions)
        {
            yield return permission;
            foreach (var descendant in permission.GetAllDescendants())
            {
                yield return descendant;
            }
        }
    }

    /// <summary>
    /// Sets a property for the group
    /// </summary>
    public PermissionGroupDefinition SetProperty(string key, object value)
    {
        Properties[key] = value;
        return this;
    }

    /// <summary>
    /// Gets a property value
    /// </summary>
    public T? GetProperty<T>(string key, T? defaultValue = default)
    {
        if (Properties.TryGetValue(key, out var value) && value is T result)
        {
            return result;
        }
        return defaultValue;
    }
} 