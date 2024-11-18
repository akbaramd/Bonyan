namespace Bonyan.Layer.Application.Services;

public interface IBonDeleteAppService<in TKey> : IBonApplicationService
{
    Task<ServiceResult> DeleteAsync(TKey id);
}