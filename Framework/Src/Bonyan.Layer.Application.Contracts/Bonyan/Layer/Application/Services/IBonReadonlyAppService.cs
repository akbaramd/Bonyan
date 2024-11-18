using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.Layer.Application.Services;



public interface
    IBonReadonlyAppService<in TKey, in TFilterDto, TDto> : IBonReadonlyAppService<TKey, TFilterDto, TDto
    , TDto>
    where TDto : IBonEntityDto<TKey>
    where TFilterDto : IBonPaginateDto
{
}

public interface IBonReadonlyAppService<in TKey, in TFilterDto, TDto, TDetailDto> : IBonApplicationService
    where TDto : IBonEntityDto<TKey>
    where TDetailDto : IBonEntityDto<TKey>
    where TFilterDto : IBonPaginateDto
{
    Task<ServiceResult<TDetailDto>> DetailAsync(TKey key);
    Task<ServiceResult<BonPaginatedResult<TDto>>> PaginatedAsync(TFilterDto paginateDto);
}