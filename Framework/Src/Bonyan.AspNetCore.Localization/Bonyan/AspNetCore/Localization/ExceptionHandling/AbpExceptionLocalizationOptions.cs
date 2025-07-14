namespace Bonyan.AspNetCore.Localization.ExceptionHandling;

public class AbpExceptionLocalizationOptions
{
    public Dictionary<string, Type> ErrorCodeNamespaceMappings { get; }

    public AbpExceptionLocalizationOptions()
    {
        ErrorCodeNamespaceMappings = new Dictionary<string, Type>();
    }

    public void MapCodeNamespace(string errorCodeNamespace, Type type)
    {
        ErrorCodeNamespaceMappings[errorCodeNamespace] = type;
    }
}
