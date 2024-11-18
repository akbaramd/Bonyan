using Bonyan.Layer.Application.Dto;

namespace Bonyan.Layer.Application.Services;



public interface IBonCrudAppService<in TKey, in TPaginateDto, TEntityDto>
    : IBonCrudAppService<TKey, TPaginateDto, TEntityDto, TEntityDto>
    where TPaginateDto : IBonPaginateDto
    where TEntityDto : IBonEntityDto<TKey>

{
}

public interface IBonCrudAppService<in TKey, in TPaginateDto, in TCreateInput, TEntityDto>
    : IBonCrudAppService<TKey, TPaginateDto, TCreateInput, TCreateInput, TEntityDto>
    where TPaginateDto : IBonPaginateDto
    where TEntityDto : IBonEntityDto<TKey>
{
}

public interface IBonCrudAppService<in TKey, in TPaginateDto, in TCreateInput, in TUpdateInput, TEntityDto>
    : IBonCrudAppService<TKey, TPaginateDto, TCreateInput, TUpdateInput, TEntityDto, TEntityDto>
    where TPaginateDto : IBonPaginateDto
    where TEntityDto : IBonEntityDto<TKey>
{
}

public interface IBonCrudAppService<in TKey, in TPaginateDto, in TCreateInput, in TUpdateInput, TGetOutputDto,
    TGetListOutputDto>
    : IBonReadonlyAppService<TKey, TPaginateDto, TGetOutputDto, TGetListOutputDto>,
        IBonCreateUpdateAppService<TKey, TCreateInput, TUpdateInput, TGetOutputDto>,
        IBonDeleteAppService<TKey>
    where TPaginateDto : IBonPaginateDto
    where TGetListOutputDto : IBonEntityDto<TKey>
    where TGetOutputDto : IBonEntityDto<TKey>
{
}