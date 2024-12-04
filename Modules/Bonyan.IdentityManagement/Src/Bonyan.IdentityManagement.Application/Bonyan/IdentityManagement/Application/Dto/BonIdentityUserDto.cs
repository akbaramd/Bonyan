using Bonyan.UserManagement.Application.Users.Dto;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Application.Dto;

public class BonIdentityUserDto : BonUserDto
{
    public List<string> Roles { get; set; }
}   


public class BonIdentityUserCreateDto 
{
    public string UserName { get; set; } = string.Empty;
    public BonUserPhoneNumber PhoneNumber { get; set; } = default!;
    public BonUserEmail Email { get; set; } = default!;
    public string Password { get; set; } = string.Empty;
    public int Status { get; set; }
    public string[] Roles { get; set; } = [];

}

public class BonIdentityUserUpdateDto
{
    public int Status { get; set; }
    public string[] Roles { get; set; } = [];
}
