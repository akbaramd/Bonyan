﻿namespace Bonyan.Layer.Application.Services;

public interface IBonUpdateAppService<in TKey, in TCreateInput, TDto>
    : IBonApplicationService
{
    Task<ServiceResult<TDto>> UpdateAsync(TKey key, TCreateInput input);
}

public interface IBonUpdateAppService<in TKey, TEntityDto>
    : IBonUpdateAppService<TKey, TEntityDto, TEntityDto>
{
}