using Bonyan.IdentityManagement.Application.Roles.Dtos;
using Bonyan.IdentityManagement.Application.Users.Dtos;

namespace Bonyan.IdentityManagement.BonWeb.Mvc.Models.Manage;

public class ManageTabRolesViewModel
{
    public Guid UserId { get; set; }
    public IReadOnlyList<UserRoleDto> UserRoles { get; set; } = new List<UserRoleDto>();
    public IReadOnlyList<RoleDto> AllRoles { get; set; } = new List<RoleDto>();
    public string? Error { get; set; }
}

public class ManageTabTokensViewModel
{
    public Guid UserId { get; set; }
    public IReadOnlyList<UserTokenDto> Tokens { get; set; } = new List<UserTokenDto>();
    public string? Error { get; set; }
}

public class ManageTabClaimsViewModel
{
    public Guid UserId { get; set; }
    public IReadOnlyList<UserClaimDto> Claims { get; set; } = new List<UserClaimDto>();
    public string? Error { get; set; }
}

public class ManageTabMetasViewModel
{
    public Guid UserId { get; set; }
    public IReadOnlyList<KeyValuePair<string, string>> Metas { get; set; } = new List<KeyValuePair<string, string>>();
}
