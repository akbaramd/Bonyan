using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Application.Dto;

namespace Bonyan.IdentityManagement.Application.Roles.Dtos;

/// <summary>
/// Role output DTO for get/detail/list.
/// </summary>
public class RoleDto : IBonEntityDto<BonRoleId>
{
    public BonRoleId Id { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public bool CanBeDeleted { get; set; }
}

/// <summary>
/// Input for creating a role. Id is the role key (e.g. "Content Manager" or "content-manager");
/// it is normalized to lowercase with spaces replaced by '-' and used as the role's Id. Title is the display name (can be Persian or any language).
/// </summary>
public class RoleCreateDto
{
    /// <summary>Role key. Will be normalized (lowercase, spaces → '-') and used as the role Id.</summary>
    public string Id { get; set; } = string.Empty;
    /// <summary>Display name of the role (e.g. "Administrator", "مدیر محتوا").</summary>
    public string Title { get; set; } = string.Empty;
    public bool CanBeDeleted { get; set; } = true;
}

/// <summary>
/// Input for updating a role.
/// </summary>
public class RoleUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public bool CanBeDeleted { get; set; }
}
