using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Domain.Entity;
using Bonyan.Layer.Domain.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.Layer.Application.Services;

public abstract class AbstractBonReadonlyAppService<TEntity, TKey, TFilterDto, TDto, TDetailDto>
    : BonApplicationService, IBonReadonlyAppService<TKey, TFilterDto, TDto, TDetailDto>
    where TEntity : class, IBonEntity<TKey>
    where TDto : class
    where TDetailDto : class
    where TFilterDto : IBonPaginateDto
{
    protected IBonReadOnlyRepository<TEntity, TKey> Repository =>
        LazyServiceProvider.LazyGetRequiredService<IBonReadOnlyRepository<TEntity, TKey>>();

    protected AbstractBonReadonlyAppService()
    {
     
    }

    public virtual async Task<ServiceResult<TDetailDto>> DetailAsync(TKey key)
    {
        var entity = await GetEntityByIdAsync(key);
        var dto = MapToDetailDto(entity);
        return ServiceResult<TDetailDto>.Success(dto);
    }

    public virtual async Task<ServiceResult<BonPaginatedResult<TDto>>> PaginatedAsync(TFilterDto paginateDto)
    {
        var query = await Repository.GetQueryableAsync();
        query = ApplyFiltering(query, paginateDto);
        var totalCount = await query.CountAsync();

        var entities = new List<TEntity>();
        if (totalCount > 0)
        {
            query = ApplyPagination(query, paginateDto);
            entities = await query.ToListAsync();
        }

        var resultData = entities.Select(MapToDto);
        var result = new BonPaginatedResult<TDto>(resultData, paginateDto.Skip, paginateDto.Take, totalCount);
        return ServiceResult<BonPaginatedResult<TDto>>.Success(result);
    }

    protected abstract Task<TEntity?> GetEntityByIdAsync(TKey id);

    protected virtual IQueryable<TEntity> ApplyFiltering(IQueryable<TEntity> query, TFilterDto input)
    {
        return query;
    }

    protected virtual IQueryable<TEntity> ApplyPagination(IQueryable<TEntity> query, TFilterDto input)
    {
        return query.Skip(input.Skip).Take(input.Take);
    }


    protected virtual TDto MapToDto(TEntity entity)
    {
        return Mapper.Map<TEntity, TDto>(entity);
    }

    protected virtual TDetailDto MapToDetailDto(TEntity entity)
    {
        return Mapper.Map<TEntity, TDetailDto>(entity);
    }
}