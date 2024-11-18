namespace Bonyan.Layer.Application.Services;

public interface IBonCreateUpdateAppService<in TKey, TEntityDto>
    : IBonCreateUpdateAppService<TKey, TEntityDto, TEntityDto, TEntityDto>
{
}

public interface IBonCreateUpdateAppService<in TKey, in TCreateUpdateInput, TEntityDto>
    : IBonCreateUpdateAppService<TKey, TCreateUpdateInput, TCreateUpdateInput, TEntityDto>
{
}

public interface IBonCreateUpdateAppService<in TKey, in TCreateInput, in TUpdateInput, TGetOutputDto>
    : IBonCreateAppService<TCreateInput, TGetOutputDto>,
        IBonUpdateAppService<TKey, TUpdateInput, TGetOutputDto>
{
}