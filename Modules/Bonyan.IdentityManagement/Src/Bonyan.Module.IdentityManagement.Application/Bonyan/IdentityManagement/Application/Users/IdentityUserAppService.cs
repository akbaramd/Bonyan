using Bonyan.IdentityManagement.Application.Base;
using Bonyan.IdentityManagement.Application.Users.Dtos;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Application.Services;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Bonyan.IdentityManagement.Application.Users;

/// <summary>
/// Identity user application service. Handles password, roles, lock/unlock, and user-role list.
/// Builds on <see cref="IdentityUserAppServiceBase"/>; user entity CRUD remains in UserManagement.
/// </summary>
public class IdentityUserAppService : IdentityUserAppServiceBase, IIdentityUserAppService
{
    public async Task<ServiceResult> ChangePasswordAsync(ChangePasswordInputDto input)
    {
        if (input == null)
            return ServiceResult.Failure("Input cannot be null.", "NullInput");

        var (user, fail) = await GetUserOrFailAsync(input.UserId);
        if (fail != null) return fail;

        return ToServiceResult(await UserManager.ChangePasswordAsync(user!, input.CurrentPassword, input.NewPassword));
    }

    public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordInputDto input)
    {
        if (input == null)
            return ServiceResult.Failure("Input cannot be null.", "NullInput");

        var (user, fail) = await GetUserOrFailAsync(input.UserId);
        if (fail != null) return fail;

        return ToServiceResult(await UserManager.ResetPasswordAsync(user!, input.NewPassword));
    }

    public async Task<ServiceResult> AssignRolesAsync(AssignRolesInputDto input)
    {
        if (input == null)
            return ServiceResult.Failure("Input cannot be null.", "NullInput");

        var (user, fail) = await GetUserOrFailAsync(input.UserId);
        if (fail != null) return fail;

        var domainResult = await UserManager.AssignRolesAsync(user!, input.RoleIds ?? Array.Empty<BonRoleId>());
        if (domainResult.IsFailure)
            return ServiceResult.Failure(domainResult.ErrorMessage ?? "Failed to assign roles.", "AssignRolesFailed");

        try
        {
            await UnitOfWorkManager.Current?.SaveChangesAsync();
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error saving role assignments for user {UserId}.", input.UserId);
            return ServiceResult.Failure($"Error saving: {ex.Message}", "SaveFailed");
        }
    }

    public async Task<ServiceResult> RemoveRoleAsync(RemoveRoleInputDto input)
    {
        if (input == null)
            return ServiceResult.Failure("Input cannot be null.", "NullInput");

        var (user, fail) = await GetUserOrFailAsync(input.UserId);
        if (fail != null) return fail;

        return ToServiceResult(await UserManager.RemoveRoleAsync(user!, input.RoleId));
    }

    public async Task<ServiceResult> LockUserAsync(LockUserInputDto input)
    {
        if (input == null)
            return ServiceResult.Failure("Input cannot be null.", "NullInput");

        var (user, fail) = await GetUserOrFailAsync(input.UserId);
        if (fail != null) return fail;

        try
        {
            user!.Lock(input.Reason);
            await UserManager.UpdateAsync(user);
            await UnitOfWorkManager.Current?.SaveChangesAsync();
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error locking user {UserId}.", input.UserId);
            return ServiceResult.Failure(ex.Message, "LockFailed");
        }
    }

    public async Task<ServiceResult> UnlockUserAsync(BonUserId userId)
    {
        var (user, fail) = await GetUserOrFailAsync(userId);
        if (fail != null) return fail;

        try
        {
            user!.Unlock();
            await UserManager.UpdateAsync(user);
            await UnitOfWorkManager.Current?.SaveChangesAsync();
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error unlocking user {UserId}.", userId);
            return ServiceResult.Failure(ex.Message, "UnlockFailed");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<UserRoleDto>>> GetUserRolesAsync(BonUserId userId)
    {
        var (user, fail) = await GetUserOrFailAsync(userId);
        if (fail != null)
            return ServiceResult<IReadOnlyList<UserRoleDto>>.Failure(fail.ErrorMessage ?? "User not found.", fail.ErrorCode ?? "NotFound");

        var rolesResult = await UserManager.GetUserRolesAsync(user!);
        if (rolesResult.IsFailure)
            return ServiceResult<IReadOnlyList<UserRoleDto>>.Failure(rolesResult.ErrorMessage ?? "Failed to get roles.", "GetRolesFailed");

        var dtos = rolesResult.Value?.Select(r => Mapper.Map<UserRoleDto>(r)).ToList() ?? new List<UserRoleDto>();
        return ServiceResult<IReadOnlyList<UserRoleDto>>.Success(dtos);
    }
}
