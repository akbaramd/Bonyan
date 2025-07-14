using JetBrains.Annotations;

namespace Bonyan.AspNetCore.Localization;

public interface IInheritedResourceTypesProvider
{
    [NotNull]
    Type[] GetInheritedResourceTypes();
}
