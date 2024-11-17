using Bonyan.Layer.Application.Dto;

namespace Bonyan.Layer.Application.Abstractions;



public interface ICrudAppService<in TKey, in TPaginateDto, TEntityDto>
    : ICrudAppService<TKey, TPaginateDto, TEntityDto, TEntityDto>
    where TPaginateDto : IBonPaginateDto
    where TEntityDto : IBonEntityDto<TKey>

{
}

public interface ICrudAppService<in TKey, in TPaginateDto, in TCreateInput, TEntityDto>
    : ICrudAppService<TKey, TPaginateDto, TCreateInput, TCreateInput, TEntityDto>
    where TPaginateDto : IBonPaginateDto
    where TEntityDto : IBonEntityDto<TKey>
{
}

public interface ICrudAppService<in TKey, in TPaginateDto, in TCreateInput, in TUpdateInput, TEntityDto>
    : ICrudAppService<TKey, TPaginateDto, TCreateInput, TUpdateInput, TEntityDto, TEntityDto>
    where TPaginateDto : IBonPaginateDto
    where TEntityDto : IBonEntityDto<TKey>
{
}

public interface ICrudAppService<in TKey, in TPaginateDto, in TCreateInput, in TUpdateInput, TGetOutputDto,
    TGetListOutputDto>
    : IBonReadonlyAppService<TKey, TPaginateDto, TGetOutputDto, TGetListOutputDto>,
        IBonCreateUpdateAppService<TKey, TCreateInput, TUpdateInput, TGetOutputDto>,
        IBonDeleteAppService<TKey>
    where TPaginateDto : IBonPaginateDto
    where TGetListOutputDto : IBonEntityDto<TKey>
    where TGetOutputDto : IBonEntityDto<TKey>
{
}