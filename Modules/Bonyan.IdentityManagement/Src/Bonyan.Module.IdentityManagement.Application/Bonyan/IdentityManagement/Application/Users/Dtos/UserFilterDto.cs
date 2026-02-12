using Bonyan.Layer.Application.Dto;

namespace Bonyan.IdentityManagement.Application.Users.Dtos;

/// <summary>
/// Filter and pagination for user list.
/// </summary>
public class UserFilterDto : BonPaginateDto
{
    public UserFilterDto()
    {
        Take = 10;
        Skip = 0;
    }

    public string? Search { get; set; }
}
