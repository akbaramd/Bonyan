using System;
using Bonyan.Modularity.Abstractions;

namespace Bonyan.Exceptions;

public class ConfigurationNotFoundException : Exception
{
    public ConfigurationNotFoundException(string name) : base($"{name} is not configured. Please configure it using context.Services.Configure<{name}>() in your main module. {nameof(IModule.OnPreConfigureAsync)} or {nameof(IModule.OnConfigureAsync)}")
    {

    }
}
