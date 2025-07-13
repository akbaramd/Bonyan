namespace Bonyan.IdentityManagement.Permissions;

/// <summary>
/// Represents a permission definition in the system
/// </summary>
public class PermissionDefinition
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public PermissionDefinition? Parent { get; set; }
    public List<PermissionDefinition> Children { get; set; } = new();
    public Dictionary<string, object> Properties { get; set; } = new();

    public PermissionDefinition()
    {
    }

    public PermissionDefinition(
        string name,
        string? displayName = null,
        string? description = null,
        PermissionDefinition? parent = null)
    {
        Name = name;
        DisplayName = displayName ?? name;
        Description = description;
        Parent = parent;
    }

    /// <summary>
    /// Adds a child permission
    /// </summary>
    public PermissionDefinition AddChild(string name, string? displayName = null, string? description = null)
    {
        var child = new PermissionDefinition(name, displayName, description, this)
        {
            GroupName = GroupName
        };
        
        Children.Add(child);
        return child;
    }

    /// <summary>
    /// Gets all descendant permissions (recursive)
    /// </summary>
    public IEnumerable<PermissionDefinition> GetAllDescendants()
    {
        foreach (var child in Children)
        {
            yield return child;
            foreach (var descendant in child.GetAllDescendants())
            {
                yield return descendant;
            }
        }
    }

    /// <summary>
    /// Checks if this permission is a descendant of the specified permission
    /// </summary>
    public bool IsDescendantOf(PermissionDefinition permission)
    {
        var current = Parent;
        while (current != null)
        {
            if (current.Name == permission.Name)
                return true;
            current = current.Parent;
        }
        return false;
    }

    /// <summary>
    /// Sets a property for the permission
    /// </summary>
    public PermissionDefinition SetProperty(string key, object value)
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