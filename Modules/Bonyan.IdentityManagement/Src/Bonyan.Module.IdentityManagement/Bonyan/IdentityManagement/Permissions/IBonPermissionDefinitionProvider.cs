namespace Bonyan.IdentityManagement.Permissions;

/// <summary>
/// Interface for providing permission definitions from modules
/// </summary>
public interface IBonPermissionDefinitionProvider
{
    /// <summary>
    /// Defines permissions for a module
    /// </summary>
    /// <param name="context">The permission definition context</param>
    void Define(IPermissionDefinitionContext context);
}

/// <summary>
/// Context for defining permissions
/// </summary>
public interface IPermissionDefinitionContext
{
    /// <summary>
    /// Gets all defined groups
    /// </summary>
    IReadOnlyList<PermissionGroupDefinition> Groups { get; }

    /// <summary>
    /// Adds a permission group
    /// </summary>
    /// <param name="name">Group name</param>
    /// <param name="displayName">Display name for UI</param>
    /// <param name="description">Optional description</param>
    /// <returns>The created group</returns>
    PermissionGroupDefinition AddGroup(string name, string? displayName = null, string? description = null);

    /// <summary>
    /// Gets a group by name
    /// </summary>
    /// <param name="name">Group name</param>
    /// <returns>The group or null if not found</returns>
    PermissionGroupDefinition? GetGroup(string name);

    /// <summary>
    /// Gets all permissions from all groups
    /// </summary>
    /// <returns>All defined permissions</returns>
    IEnumerable<PermissionDefinition> GetAllPermissions();
}

/// <summary>
/// Implementation of permission definition context
/// </summary>
public class PermissionDefinitionContext : IPermissionDefinitionContext
{
    private readonly List<PermissionGroupDefinition> _groups = new();

    public IReadOnlyList<PermissionGroupDefinition> Groups => _groups.AsReadOnly();

    public PermissionGroupDefinition AddGroup(string name, string? displayName = null, string? description = null)
    {
        var existingGroup = GetGroup(name);
        if (existingGroup != null)
        {
            return existingGroup;
        }

        var group = new PermissionGroupDefinition(name, displayName, description);
        _groups.Add(group);
        return group;
    }

    public PermissionGroupDefinition? GetGroup(string name)
    {
        return _groups.FirstOrDefault(g => g.Name == name);
    }

    public IEnumerable<PermissionDefinition> GetAllPermissions()
    {
        foreach (var group in _groups)
        {
            foreach (var permission in group.GetAllPermissions())
            {
                yield return permission;
            }
        }
    }
}