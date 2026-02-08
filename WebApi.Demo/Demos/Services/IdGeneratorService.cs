using Bonyan.DependencyInjection;

namespace WebApi.Demo.Demos.Services;

/// <summary>
/// Transient id generator registered via [BonTransient] attribute.
/// </summary>
[BonTransient(serviceTypes: typeof(IIdGeneratorService))]
public class IdGeneratorService : IIdGeneratorService
{
    private static int _counter;

    public int Next() => Interlocked.Increment(ref _counter);
    public string NextString() => $"id-{Next()}";
}
