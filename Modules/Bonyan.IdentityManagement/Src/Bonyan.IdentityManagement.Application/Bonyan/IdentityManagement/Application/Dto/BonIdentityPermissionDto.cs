using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Application.Dto;

namespace Bonyan.IdentityManagement.Application.Dto;

public class BonIdentityPermissionDto : BonAggregateRootDto<BonPermissionId>
{
    public string Title { get; set; } = string.Empty;
}