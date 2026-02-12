using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Application.Users.Dtos;

/// <summary>
/// DTO for user list (index table).
/// </summary>
public class UserListDto
{
    public BonUserId Id { get; set; } = null!;
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsLocked { get; set; }
}
