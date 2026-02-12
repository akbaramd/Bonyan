using Bonyan.IdentityManagement.Application.Users.Dtos;
using Bonyan.Layer.Application.Services;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Application.Users;

/// <summary>
/// Application service for identity-specific user operations (password, roles, claims, tokens, lock). Returns <see cref="ServiceResult"/>.
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
    Task<ServiceResult<UserListDto?>> GetUserManageInfoAsync(BonUserId userId);
    Task<ServiceResult<IReadOnlyList<UserClaimDto>>> GetUserClaimsAsync(BonUserId userId);
    Task<ServiceResult> AddUserClaimAsync(BonUserId userId, string claimType, string claimValue, string? issuer = null);
    Task<ServiceResult> RemoveUserClaimAsync(BonUserId userId, string claimType, string claimValue);
    Task<ServiceResult<IReadOnlyList<UserTokenDto>>> GetUserTokensAsync(BonUserId userId);
    Task<ServiceResult> RemoveUserTokenAsync(BonUserId userId, string tokenType);
}
