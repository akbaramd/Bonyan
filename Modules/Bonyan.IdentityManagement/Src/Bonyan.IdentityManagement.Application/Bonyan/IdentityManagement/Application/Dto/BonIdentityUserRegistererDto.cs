using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.UserManagement.Application.Users.Dto;

namespace Bonyan.IdentityManagement.Application.Dto;

public class BonIdentityUserRegistererDto : BonUserCreateDto
{
    public string Password { get; set; }
}