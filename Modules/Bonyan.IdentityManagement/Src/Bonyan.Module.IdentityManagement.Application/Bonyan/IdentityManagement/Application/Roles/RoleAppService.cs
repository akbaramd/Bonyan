using Bonyan.IdentityManagement.Application.Base;
using Bonyan.IdentityManagement.Application.Roles.Dtos;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.DomainServices;
using Bonyan.IdentityManagement.Domain.Roles.Repositories;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bonyan.IdentityManagement.Application.Roles;

/// <summary>
/// Role application service. Base CRUD from <see cref="AbstractIdentityCrudAppService{TEntity,TKey,TFilterDto,TGetOutputDto,TCreateInput,TUpdateInput}"/>,
/// implemented with <see cref="IBonIdentityRoleManager"/> for create/update/delete.
/// </summary>
public class RoleAppService : AbstractIdentityCrudAppService<BonIdentityRole, BonRoleId, RoleFilterDto, RoleDto, RoleCreateDto, RoleUpdateDto>, IRoleAppService
{
    protected IBonIdentityRoleManager RoleManager => LazyServiceProvider.LazyGetRequiredService<IBonIdentityRoleManager>();

    public override async Task<ServiceResult<RoleDto>> CreateAsync(RoleCreateDto input)
    {
        if (input == null)
            return ServiceResult<RoleDto>.Failure("Input cannot be null.", "NullInput");

        var normalizedId = RoleKeyNormalizer.Normalize(input.Id);
        if (string.IsNullOrEmpty(normalizedId))
            return ServiceResult<RoleDto>.Failure("Role Id (key) cannot be empty after normalization.", "InvalidId");

        try
        {
            var id = BonRoleId.NewId(normalizedId);
            var role = new BonIdentityRole(id, input.Title, input.CanBeDeleted);
            var domainResult = await RoleManager.CreateRoleAsync(role);
            if (domainResult.IsFailure)
                return ServiceResult<RoleDto>.Failure(domainResult.ErrorMessage ?? "Failed to create role.", "CreateFailed");
            await UnitOfWorkManager.Current?.SaveChangesAsync();
            return ServiceResult<RoleDto>.Success(MapToDto(role));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating role.");
            return ServiceResult<RoleDto>.Failure($"Error creating role: {ex.Message}", "CreateFailed");
        }
    }

    public override async Task<ServiceResult<RoleDto>> UpdateAsync(BonRoleId id, RoleUpdateDto input)
    {
        if (input == null)
            return ServiceResult<RoleDto>.Failure("Input cannot be null.", "NullInput");

        var findResult = await RoleManager.FindRoleByIdAsync(id.Value);
        if (findResult.IsFailure || findResult.Value == null)
            return ServiceResult<RoleDto>.Failure("Role not found.", "NotFound");

        var role = findResult.Value;
        try
        {
            role.UpdateTitle(input.Title);
            if (input.CanBeDeleted) role.MarkAsDeletable(); else role.MarkAsNonDeletable();
            var domainResult = await RoleManager.UpdateRoleAsync(role);
            if (domainResult.IsFailure)
                return ServiceResult<RoleDto>.Failure(domainResult.ErrorMessage ?? "Failed to update role.", "UpdateFailed");
            await UnitOfWorkManager.Current?.SaveChangesAsync();
            return ServiceResult<RoleDto>.Success(MapToDto(role));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating role {RoleId}.", id);
            return ServiceResult<RoleDto>.Failure($"Error updating role: {ex.Message}", "UpdateFailed");
        }
    }

    public override async Task<ServiceResult> DeleteAsync(BonRoleId id)
    {
        var findResult = await RoleManager.FindRoleByIdAsync(id.Value);
        if (findResult.IsFailure || findResult.Value == null)
            return ServiceResult.Failure("Role not found.", "NotFound");

        var role = findResult.Value;
        if (!role.CanBeDeleted)
            return ServiceResult.Failure("This role cannot be deleted because it is marked as non-deletable.", "CannotDelete");

        try
        {
            var domainResult = await RoleManager.DeleteRoleAsync(role);
            if (domainResult.IsFailure)
                return ServiceResult.Failure(domainResult.ErrorMessage ?? "Failed to delete role.", "DeleteFailed");
            await UnitOfWorkManager.Current?.SaveChangesAsync();
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting role {RoleId}.", id);
            return ServiceResult.Failure($"Error deleting role: {ex.Message}", "DeleteFailed");
        }
    }

    protected override IQueryable<BonIdentityRole> ApplyFiltering(IQueryable<BonIdentityRole> query, RoleFilterDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Search)) return query;
        var search = input.Search.Trim().ToLowerInvariant();
        return query.Where(r => r.Title != null && r.Title.ToLower().Contains(search));
    }
}
