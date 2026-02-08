namespace WebApi.Demo.Demos.Services;

/// <summary>
/// Application clock for testing [BonSingleton] registration.
/// </summary>
public interface IAppClockService
{
    DateTime UtcNow { get; }
    string InstanceId { get; }
}
