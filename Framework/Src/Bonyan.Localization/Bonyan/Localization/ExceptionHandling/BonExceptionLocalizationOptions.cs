namespace Bonyan.Localization.ExceptionHandling;

public class BonExceptionLocalizationOptions
{
    public Dictionary<string, Type> ErrorCodeNamespaceMappings { get; }

    public BonExceptionLocalizationOptions()
    {
        ErrorCodeNamespaceMappings = new Dictionary<string, Type>();
    }

    public void MapCodeNamespace(string errorCodeNamespace, Type type)
    {
        ErrorCodeNamespaceMappings[errorCodeNamespace] = type;
    }
}
