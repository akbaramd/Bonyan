using Bonyan.IdentityManagement.Application.Users.Dtos;
using Bonyan.Layer.Application.Services;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Application.Users;

/// <summary>
/// Application service for identity-specific user operations (password, roles, lock). Returns <see cref="ServiceResult"/>.
/// User CRUD remains in UserManagement; this service covers identity only.
/// </summary>
public interface IIdentityUserAppService
{
    Task<ServiceResult> ChangePasswordAsync(ChangePasswordInputDto input);
    Task<ServiceResult> ResetPasswordAsync(ResetPasswordInputDto input);
    Task<ServiceResult> AssignRolesAsync(AssignRolesInputDto input);
    Task<ServiceResult> RemoveRoleAsync(RemoveRoleInputDto input);
    Task<ServiceResult> LockUserAsync(LockUserInputDto input);
    Task<ServiceResult> UnlockUserAsync(BonUserId userId);
    Task<ServiceResult<IReadOnlyList<UserRoleDto>>> GetUserRolesAsync(BonUserId userId);
}
