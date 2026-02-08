using Bonyan.DependencyInjection;

namespace WebApi.Demo.Demos.Services;

/// <summary>
/// Singleton clock service registered via [BonSingleton] attribute.
/// </summary>
[BonSingleton(serviceTypes: typeof(IAppClockService))]
public class AppClockService : IAppClockService
{
    public DateTime UtcNow => DateTime.UtcNow;
    public string InstanceId { get; } = Guid.NewGuid().ToString("N")[..8];
}
