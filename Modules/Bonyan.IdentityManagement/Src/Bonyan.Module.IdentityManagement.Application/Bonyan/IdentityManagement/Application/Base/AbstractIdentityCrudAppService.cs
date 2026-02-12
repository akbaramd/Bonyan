using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Entity;
using Bonyan.Layer.Domain.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bonyan.IdentityManagement.Application.Base;

/// <summary>
/// Base CRUD application service for identity aggregates. Provides Detail and Paginated from read-only repository;
/// Create, Update, Delete are abstract so subclasses implement them using domain managers (not repository).
/// Follows the same contract as <see cref="Bonyan.Layer.Application.Services.AbstractCrudAppService{TEntity,TKey,TPaginateDto,TGetOutputDto,TGetListOutputDto,TCreateInput,TUpdateInput}"/>.
/// </summary>
public abstract class AbstractIdentityCrudAppService<TEntity, TKey, TFilterDto, TGetOutputDto, TCreateInput, TUpdateInput>
    : BonApplicationService,
        IBonCrudAppService<TKey, TFilterDto, TCreateInput, TUpdateInput, TGetOutputDto>
    where TEntity : class, IBonEntity<TKey>
    where TFilterDto : IBonPaginateDto
    where TGetOutputDto : class
    where TCreateInput : class
    where TUpdateInput : class
{
    protected IBonReadOnlyRepository<TEntity, TKey> ReadOnlyRepository =>
        LazyServiceProvider.LazyGetRequiredService<IBonReadOnlyRepository<TEntity, TKey>>();

    protected AbstractIdentityCrudAppService()
    {
    }

    #region Read (from framework pattern)

    public virtual async Task<ServiceResult<TGetOutputDto>> DetailAsync(TKey key)
    {
        try
        {
            var entity = await GetEntityByIdAsync(key);
            if (entity == null)
                return ServiceResult<TGetOutputDto>.Failure("Entity not found.", "NotFound");
            return ServiceResult<TGetOutputDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting detail for key {Key}.", key);
            return ServiceResult<TGetOutputDto>.Failure($"Error loading detail: {ex.Message}", "DetailFailed");
        }
    }

    public virtual async Task<ServiceResult<BonPaginatedResult<TGetOutputDto>>> PaginatedAsync(TFilterDto paginateDto)
    {
        try
        {
            var query = await ReadOnlyRepository.GetQueryableAsync();
            query = ApplyFiltering(query, paginateDto);
            var totalCount = await query.CountAsync();

            var entities = totalCount > 0
                ? await ApplyPagination(query, paginateDto).ToListAsync()
                : new List<TEntity>();

            var resultData = entities.Select(MapToDto);
            var result = new BonPaginatedResult<TGetOutputDto>(resultData, paginateDto.Skip, paginateDto.Take, totalCount);
            return ServiceResult<BonPaginatedResult<TGetOutputDto>>.Success(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading paginated list.");
            return ServiceResult<BonPaginatedResult<TGetOutputDto>>.Failure($"Error loading list: {ex.Message}", "PaginatedFailed");
        }
    }

    #endregion

    #region Write (implement in subclass using domain manager)

    public abstract Task<ServiceResult<TGetOutputDto>> CreateAsync(TCreateInput input);
    public abstract Task<ServiceResult<TGetOutputDto>> UpdateAsync(TKey id, TUpdateInput input);
    public abstract Task<ServiceResult> DeleteAsync(TKey id);

    #endregion

    #region Helpers (override in subclass)

    protected virtual Task<TEntity?> GetEntityByIdAsync(TKey id) =>
        ReadOnlyRepository.FindOneAsync(e => e.Id != null && e.Id.Equals(id));

    protected virtual IQueryable<TEntity> ApplyFiltering(IQueryable<TEntity> query, TFilterDto input) => query;

    protected virtual IQueryable<TEntity> ApplyPagination(IQueryable<TEntity> query, TFilterDto input) =>
        query.Skip(input.Skip).Take(input.Take);

    protected virtual TGetOutputDto MapToDto(TEntity entity) => Mapper.Map<TEntity, TGetOutputDto>(entity);

    #endregion
}
