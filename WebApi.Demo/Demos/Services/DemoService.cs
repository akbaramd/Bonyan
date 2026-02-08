using Bonyan.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApi.Demo.Demos.Models;

namespace WebApi.Demo.Demos.Services;

/// <summary>
/// Demo service registered via [BonScoped] attribute (conventional registration).
/// </summary>
[BonScoped(serviceTypes: typeof(IDemoService))]
public class DemoService : IDemoService
{
    private readonly List<DemoItem> _items = new();
    private int _nextId = 1;
    private readonly ILogger<DemoService> _logger;

    public DemoService(ILogger<DemoService> logger)
    {
        _logger = logger;
        _items.Add(new DemoItem { Id = _nextId++, Key = "welcome", Value = "Hello from attributed service", CreatedAt = DateTime.UtcNow });
    }

    public Task<IReadOnlyList<DemoItem>> GetItemsAsync()
    {
        _logger.LogDebug("DemoService.GetItemsAsync");
        return Task.FromResult<IReadOnlyList<DemoItem>>(_items);
    }

    public Task<DemoItem?> GetByIdAsync(int id)
    {
        return Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
    }

    public Task<DemoItem> CreateAsync(string key, string value)
    {
        var item = new DemoItem { Id = _nextId++, Key = key, Value = value, CreatedAt = DateTime.UtcNow };
        _items.Add(item);
        return Task.FromResult(item);
    }
}
