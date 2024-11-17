using Bonyan.Layer.Application.Abstractions;
using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Domain.Entity;
using Bonyan.Layer.Domain.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.Layer.Application.Services;

public class CrudAppService<TEntity, TKey, TGetOutputDto>
    : CrudAppService<TEntity, TKey, BonPaginateDto, TGetOutputDto>
    where TEntity : class, IBonEntity<TKey>
    where TGetOutputDto : class, IBonEntityDto<TKey>
{
    public CrudAppService(IBonRepository<TEntity, TKey> repository) : base(repository)
    {
    }
}

public class CrudAppService<TEntity, TKey, TPaginateDto, TGetOutputDto>
    : CrudAppService<TEntity, TKey, TPaginateDto, TGetOutputDto, TGetOutputDto>
    where TEntity : class, IBonEntity<TKey>
    where TGetOutputDto : class, IBonEntityDto<TKey>
    where TPaginateDto : BonPaginateDto
{
    public CrudAppService(IBonRepository<TEntity, TKey> repository) : base(repository)
    {
    }
}

public class CrudAppService<TEntity, TKey, TPaginateDto, TGetOutputDto, TGetListOutputDto>
    : CrudAppService<TEntity, TKey, TPaginateDto, TGetOutputDto, TGetListOutputDto, TGetOutputDto>
    where TEntity : class, IBonEntity<TKey>
    where TGetListOutputDto : IBonEntityDto<TKey>
    where TGetOutputDto : class, IBonEntityDto<TKey>
    where TPaginateDto : BonPaginateDto
{
    public CrudAppService(IBonRepository<TEntity, TKey> repository) : base(repository)
    {
    }
}

public class CrudAppService<TEntity, TKey, TPaginateDto, TGetOutputDto, TGetListOutputDto, TCreateUpdateInput>
    : CrudAppService<TEntity, TKey, TPaginateDto, TGetOutputDto, TGetListOutputDto, TCreateUpdateInput,
        TCreateUpdateInput>
    where TEntity : class, IBonEntity<TKey>
    where TGetListOutputDto : IBonEntityDto<TKey>
    where TGetOutputDto : IBonEntityDto<TKey>
    where TCreateUpdateInput : class
    where TPaginateDto : BonPaginateDto
{
    public CrudAppService(IBonRepository<TEntity, TKey> repository) : base(repository)
    {
    }
}

public class CrudAppService<TEntity, TKey, TPaginateDto, TGetOutputDto, TGetListOutputDto, TCreateInput, TUpdateInput>
    : AbstractCrudAppService<TEntity, TKey, TPaginateDto, TGetOutputDto, TGetListOutputDto, TCreateInput, TUpdateInput>
    where TEntity : class, IBonEntity<TKey>
    where TGetListOutputDto : IBonEntityDto<TKey>
    where TGetOutputDto : IBonEntityDto<TKey>
    where TCreateInput : class
    where TUpdateInput : class
    where TPaginateDto : IBonPaginateDto
{
    public CrudAppService(IBonRepository<TEntity, TKey> repository) : base(repository)
    {
    }


    protected override Task<TEntity?> GetEntityByIdAsync(TKey id)
    {
        return Repository.FindOneAsync(x => x.Id != null && x.Id.Equals(id));
    }
}