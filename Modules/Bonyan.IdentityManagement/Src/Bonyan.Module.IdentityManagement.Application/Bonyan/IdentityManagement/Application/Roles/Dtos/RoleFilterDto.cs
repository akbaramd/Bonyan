using Bonyan.Layer.Application.Dto;

namespace Bonyan.IdentityManagement.Application.Roles.Dtos;

/// <summary>
/// Filter and pagination for role list.
/// </summary>
public class RoleFilterDto : BonPaginateDto
{
    public RoleFilterDto()
    {
        Take = 10;
        Skip = 0;
    }
    public string? Search { get; set; }
}
