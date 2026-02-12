using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Application.Users.Dtos;

/// <summary>
/// Input for change-password operation.
/// </summary>
public class ChangePasswordInputDto
{
    public BonUserId UserId { get; set; } = null!;
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

/// <summary>
/// Input for reset-password (admin) operation.
/// </summary>
public class ResetPasswordInputDto
{
    public BonUserId UserId { get; set; } = null!;
    public string NewPassword { get; set; } = string.Empty;
}

/// <summary>
/// Input for assign-roles operation.
/// </summary>
public class AssignRolesInputDto
{
    public BonUserId UserId { get; set; } = null!;
    public IReadOnlyList<BonRoleId> RoleIds { get; set; } = Array.Empty<BonRoleId>();
}

/// <summary>
/// Input for remove-role operation.
/// </summary>
public class RemoveRoleInputDto
{
    public BonUserId UserId { get; set; } = null!;
    public BonRoleId RoleId { get; set; } = null!;
}

/// <summary>
/// Input for lock-user operation.
/// </summary>
public class LockUserInputDto
{
    public BonUserId UserId { get; set; } = null!;
    public string? Reason { get; set; }
}

/// <summary>
/// Role DTO for user-roles list.
/// </summary>
public class UserRoleDto
{
    public BonRoleId Id { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
}
