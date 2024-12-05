using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Application.Dto;

namespace Bonyan.IdentityManagement.Application.Dto;

public class BonIdentityRoleDto : BonAggregateRootDto<string>
{
    public string Title { get; set; } = string.Empty;

    public List<BonIdentityPermissionDto> Permissions { get; set; } = [];
}


public class BonIdentityRoleCreateDto 
{
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string[] Permissions { get; set; } = [];

}

public class BonIdentityRoleUpdateDto 
{
    public string Title { get; set; } = string.Empty;
    public string[] Permissions { get; set; } = [];

}