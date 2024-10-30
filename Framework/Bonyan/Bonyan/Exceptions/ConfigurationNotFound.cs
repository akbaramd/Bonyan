using Bonyan.Modularity.Abstractions;

namespace Bonyan.Exceptions;

public class ConfigurationNotFoundException<TType> : Exception
{
    public ConfigurationNotFoundException() : base($"{nameof(TType)} is not configured. Please configure it using context.Services.Configure<{nameof(TType)}>() in your main module. {nameof(IModule.OnPreConfigureAsync)} or {nameof(IModule.OnConfigureAsync)}")
    {

    }
}
