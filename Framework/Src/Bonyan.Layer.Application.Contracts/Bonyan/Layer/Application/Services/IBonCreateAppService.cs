namespace Bonyan.Layer.Application.Services;

public interface IBonCreateAppService<TEntityDto>
    : IBonCreateAppService<TEntityDto, TEntityDto>
{
}

public interface IBonCreateAppService< in TCreateInput,TDto>
    : IBonApplicationService
{
    Task<ServiceResult<TDto>> CreateAsync(TCreateInput input);
}