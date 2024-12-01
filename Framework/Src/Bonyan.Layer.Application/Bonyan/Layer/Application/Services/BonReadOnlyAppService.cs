using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Exceptions;
using Bonyan.Layer.Domain.Entity;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.Layer.Application.Services;

public abstract class BonReadonlyAppService<TEntity,TKey, TEntityDto >
    : BonReadonlyAppService<TEntity,TKey, BonPaginateDto, TEntityDto, TEntityDto> where TEntity : class, IBonEntity<TKey> where TEntityDto : IBonEntityDto<TKey>

{
    protected BonReadonlyAppService(IBonReadOnlyRepository<TEntity, TKey> repository)
        : base(repository)
    {

    }
}

public abstract class BonReadonlyAppService<TEntity, TKey, TGetListInput, TEntityDto>
    : BonReadonlyAppService<TEntity,  TKey, TGetListInput,TEntityDto, TEntityDto> 
    where TGetListInput : BonPaginateDto 
    where TEntity : class, IBonEntity<TKey> 
    where TEntityDto : IBonEntityDto<TKey>

{
    protected BonReadonlyAppService(IBonReadOnlyRepository<TEntity, TKey> repository)
        : base(repository)
    {

    }
}

public class BonReadonlyAppService<TEntity, TKey,TFilterDto, TDto, TDetailDto>
    : AbstractBonReadonlyAppService<TEntity, TKey,TFilterDto, TDto, TDetailDto>
    where TEntity : class, IBonEntity<TKey>
    where TDto : IBonEntityDto<TKey>
    where TDetailDto : IBonEntityDto<TKey>
    where TFilterDto : BonPaginateDto
{
    public BonReadonlyAppService(IBonReadOnlyRepository<TEntity,TKey> repository)
        : base(repository)
    {
    }

    protected override async Task<TEntity?> GetEntityByIdAsync(TKey id)
    {
        var entity = await Repository.FindOneAsync(e => e.Id.Equals(id));
        if (entity == null)
        {
            throw new BonEntityNotFoundException(typeof(TEntity), id);
        }

        return entity;
    }

  
}