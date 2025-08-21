using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.Novino.Module.RoleManagement.Areas.RoleManagement.ViewModels;

/// <summary>
/// View model for user role details in role management module
/// </summary>
public class UserRoleDetailsViewModel
{
    /// <summary>
    /// User identification
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User status
    /// </summary>
    public bool IsActive { get; set; }
    public bool IsLocked { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Role-related data
    /// </summary>
    public List<UserRoleViewModel> Roles { get; set; } = new();
    public int RolesCount => Roles.Count;

    /// <summary>
    /// Role management permissions
    /// </summary>
    public bool CanViewRoles { get; set; }
    public bool CanManageRoles { get; set; }
    public bool CanAssignRoles { get; set; }
    public bool CanRemoveRoles { get; set; }
    public bool CanViewRoleDetails { get; set; }

    /// <summary>
    /// Available roles for assignment
    /// </summary>
    public List<AvailableRoleViewModel> AvailableRoles { get; set; } = new();

    /// <summary>
    /// Role management context
    /// </summary>
    public string CurrentUserId { get; set; } = string.Empty;
    public string CurrentUserName { get; set; } = string.Empty;
    public DateTime LoadedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// View model for user role information
/// </summary>
public class UserRoleViewModel
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime AssignedAt { get; set; }
    public string AssignedBy { get; set; } = string.Empty;
    public int ClaimsCount { get; set; }
    public List<string> Permissions { get; set; } = new();
    public bool CanRemove { get; set; }
    public bool CanViewDetails { get; set; }
}

/// <summary>
/// View model for available roles that can be assigned
/// </summary>
public class AvailableRoleViewModel
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int UsersCount { get; set; }
    public int ClaimsCount { get; set; }
    public bool IsAlreadyAssigned { get; set; }
    public bool CanAssign { get; set; }
    public bool CanViewDetails { get; set; }
}

/// <summary>
/// View model for role assignment operation
/// </summary>
public class RoleAssignmentViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string AssignedBy { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}

/// <summary>
/// View model for role removal operation
/// </summary>
public class RoleRemovalViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string RemovedBy { get; set; } = string.Empty;
    public DateTime RemovedAt { get; set; } = DateTime.UtcNow;
    public string? Reason { get; set; }
} 