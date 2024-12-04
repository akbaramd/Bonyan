using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Application.Dto;

namespace Bonyan.IdentityManagement.Application.Dto;

public class BonIdentityPermissionDto : BonAggregateRootDto<string>
{
    public string Title { get; set; } = string.Empty;
}