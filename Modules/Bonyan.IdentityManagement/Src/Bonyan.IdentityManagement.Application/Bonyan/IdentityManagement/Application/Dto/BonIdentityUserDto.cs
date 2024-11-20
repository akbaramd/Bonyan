using Bonyan.UserManagement.Application.Users.Dto;

namespace Bonyan.IdentityManagement.Application.Dto;

public class BonIdentityUserDto : BonUserDto
{
    public IReadOnlyList<string> Roles { get; set; }
}   