using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Domain.Repository.Abstractions;
using Bonyan.Layer.Domain.Specification.Abstractions;

namespace Bonyan.Layer.Application.Abstractions;



public interface
    IBonReadonlyAppService<in TKey, in TPaginatedDto, TDto> : IBonReadonlyAppService<TKey, TPaginatedDto, TDto
    , TDto>
    where TDto : IBonEntityDto<TKey>
    where TPaginatedDto : IBonPaginateDto
{
}

public interface IBonReadonlyAppService<in TKey, in TPaginatedDto, TDto, TDetailDto> : IBonApplicationService
    where TDto : IBonEntityDto<TKey>
    where TDetailDto : IBonEntityDto<TKey>
    where TPaginatedDto : IBonPaginateDto
{
    Task<ServiceResult<TDetailDto>> DetailAsync(TKey key);
    Task<ServiceResult<BonPaginatedResult<TDto>>> PaginatedAsync(TPaginatedDto paginateDto);
}