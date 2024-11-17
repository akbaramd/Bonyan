namespace Bonyan.Layer.Application.Abstractions;

public interface IBonDeleteAppService<in TKey> : IBonApplicationService
{
    Task<ServiceResult> DeleteAsync(TKey id);
}