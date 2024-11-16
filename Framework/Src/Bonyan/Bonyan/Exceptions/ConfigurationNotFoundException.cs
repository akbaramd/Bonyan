using Bonyan.Modularity.Abstractions;

namespace Bonyan.Exceptions;

public class ConfigurationNotFoundException<TType> : BusinessException
{
    public ConfigurationNotFoundException(string code = $"ConfigurationNotFoundException:{nameof(TType)}") : base(code:code,message:$"{nameof(TType)} is not configured. Please configure it using context.Services.Configure<{nameof(TType)}>() in your main module. {nameof(IBonModule.OnPreConfigureAsync)} or {nameof(IBonModule.OnConfigureAsync)}")
    {

    }
}
