using WebApi.Demo.Demos.Models;

namespace WebApi.Demo.Demos.Services;

/// <summary>
/// Demo service interface for testing [BonScoped] registration.
/// </summary>
public interface IDemoService
{
    Task<IReadOnlyList<DemoItem>> GetItemsAsync();
    Task<DemoItem?> GetByIdAsync(int id);
    Task<DemoItem> CreateAsync(string key, string value);
}
