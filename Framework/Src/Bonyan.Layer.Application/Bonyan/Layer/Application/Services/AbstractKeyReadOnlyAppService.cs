using Bonyan.Layer.Application.Abstractions;
using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Domain.Entity;
using Bonyan.Layer.Domain.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.Layer.Application.Services;

public abstract class AbstractBonReadonlyAppService<TEntity, TKey,TPaginatedDto, TDto, TDetailDto>
    : BonApplicationService, IBonReadonlyAppService<TKey, TPaginatedDto, TDto, TDetailDto>
    where TEntity : class, IBonEntity<TKey>
    where TDto : IBonEntityDto<TKey>
    where TDetailDto : IBonEntityDto<TKey>
    where TPaginatedDto : IBonPaginateDto
{
    protected IBonReadOnlyRepository<TEntity,TKey> Repository { get; }

    protected AbstractBonReadonlyAppService(IBonReadOnlyRepository<TEntity,TKey> readOnlyRepository)
    {
        Repository = readOnlyRepository;
    }

    public virtual async Task<ServiceResult<TDetailDto>> DetailAsync(TKey key)
    {
        var entity = await GetEntityByIdAsync(key);
        var dto = MapToDetailDto(entity);
        return ServiceResult<TDetailDto>.Success(dto);
    }

    public virtual async Task<ServiceResult<BonPaginatedResult<TDto>>> PaginatedAsync(TPaginatedDto paginateDto)
    {
        var query = await CreateFilteredQueryAsync(paginateDto);
        var totalCount = await query.CountAsync();

        var entities = new List<TEntity>();
        if (totalCount > 0)
        {
            query = ApplyPaging(query, paginateDto);
            entities = await query.ToListAsync();
        }

        var dtos =  MapToDto(entities);
        var result = new BonPaginatedResult<TDto>( dtos,paginateDto.Skip,paginateDto.Take,totalCount);
        return ServiceResult<BonPaginatedResult<TDto>>.Success(result);
    }

    protected abstract Task<TEntity?> GetEntityByIdAsync(TKey id);

    protected virtual Task<IQueryable<TEntity>> CreateFilteredQueryAsync(TPaginatedDto input)
    {
        return Repository.GetQueryableAsync();
    }

    protected virtual IQueryable<TEntity> ApplyPaging(IQueryable<TEntity> query, TPaginatedDto input)
    {
        return query.Skip(input.Skip).Take(input.Take);
    }

    protected virtual TDto MapToDto(TEntity entity)
    {
        return Mapper.Map<TEntity, TDto>(entity);
    }
    protected virtual List<TDto> MapToDto(List<TEntity> entity)
    {
        return Mapper.Map<List<TEntity>, List<TDto>>(entity);
    }
     protected virtual TDetailDto MapToDetailDto(TEntity entity)
    {
        return Mapper.Map<TEntity, TDetailDto>(entity);
    }



}
