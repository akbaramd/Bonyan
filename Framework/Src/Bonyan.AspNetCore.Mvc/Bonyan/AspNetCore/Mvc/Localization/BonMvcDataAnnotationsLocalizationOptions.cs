using System.Reflection;
using JetBrains.Annotations;

namespace Bonyan.AspNetCore.Mvc.Localization;

public class BonMvcDataAnnotationsLocalizationOptions
{
    public IDictionary<Assembly, Type> AssemblyResources { get; }

    public BonMvcDataAnnotationsLocalizationOptions()
    {
        AssemblyResources = new Dictionary<Assembly, Type>();
    }

    public void AddAssemblyResource(
        [NotNull] Type resourceType,
        params Assembly[] assemblies)
    {
        if (assemblies.IsNullOrEmpty())
        {
            assemblies = new[] { resourceType.Assembly };
        }

        foreach (var assembly in assemblies)
        {
            AssemblyResources[assembly] = resourceType;
        }
    }
}