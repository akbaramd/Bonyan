using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Domain.Entity;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.Layer.Application.Services;

public class BonCrudAppService<TEntity, TKey, TGetOutputDto>
    : BonCrudAppService<TEntity, TKey, BonPaginateDto, TGetOutputDto>
    where TEntity : class, IBonEntity<TKey>
    where TGetOutputDto : class, IBonEntityDto<TKey>
{
   
}

public class BonCrudAppService<TEntity, TKey, TPaginateDto, TGetOutputDto>
    : BonCrudAppService<TEntity, TKey, TPaginateDto, TGetOutputDto, TGetOutputDto>
    where TEntity : class, IBonEntity<TKey>
    where TGetOutputDto : class
    where TPaginateDto : BonPaginateDto
{

}

public class BonCrudAppService<TEntity, TKey, TPaginateDto, TGetOutputDto, TGetListOutputDto>
    : BonCrudAppService<TEntity, TKey, TPaginateDto, TGetOutputDto, TGetListOutputDto, TGetOutputDto>
    where TEntity : class, IBonEntity<TKey>
    where TGetListOutputDto : class
    where TGetOutputDto : class
    where TPaginateDto : BonPaginateDto
{
   
}

public class BonCrudAppService<TEntity, TKey, TPaginateDto, TGetOutputDto, TGetListOutputDto, TCreateUpdateInput>
    : BonCrudAppService<TEntity, TKey, TPaginateDto, TGetOutputDto, TGetListOutputDto, TCreateUpdateInput,
        TCreateUpdateInput>
    where TEntity : class, IBonEntity<TKey>
    where TGetListOutputDto : class
    where TGetOutputDto : class
    where TCreateUpdateInput : class
    where TPaginateDto : BonPaginateDto
{

}

public class BonCrudAppService<TEntity, TKey, TPaginateDto, TGetOutputDto, TGetListOutputDto, TCreateInput, TUpdateInput>
    : AbstractCrudAppService<TEntity, TKey, TPaginateDto, TGetOutputDto, TGetListOutputDto, TCreateInput, TUpdateInput>
    where TEntity : class, IBonEntity<TKey>
    where TGetListOutputDto : class
    where TGetOutputDto : class
    where TCreateInput : class
    where TUpdateInput : class
    where TPaginateDto : IBonPaginateDto
{


    protected override Task<TEntity?> GetEntityByIdAsync(TKey id)
    {
        return Repository.FindOneAsync(x => x.Id != null && x.Id.Equals(id));
    }
}