using Autofac;

namespace Autofac.Extensions.DependencyInjection;

/// <summary>
/// Tries Autofac first, then falls back to the default MS DI provider.
/// Used so excluded framework services (e.g. DiagnosticSource/DiagnosticListener) are resolved from MS DI.
/// </summary>
internal sealed class ChainedServiceProvider : IServiceProvider
{
    private readonly IComponentContext _autofacContext;
    private readonly IServiceProvider _fallback;

    public ChainedServiceProvider(IComponentContext autofacContext, IServiceProvider fallback)
    {
        _autofacContext = autofacContext ?? throw new ArgumentNullException(nameof(autofacContext));
        _fallback = fallback ?? throw new ArgumentNullException(nameof(fallback));
    }

    public object? GetService(Type serviceType)
    {
        if (serviceType == null)
            return null;

        if (_autofacContext.TryResolve(serviceType, out var instance))
            return instance;

        return _fallback.GetService(serviceType);
    }
}
