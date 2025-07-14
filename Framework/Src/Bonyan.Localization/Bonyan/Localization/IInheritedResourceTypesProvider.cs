using JetBrains.Annotations;

namespace Bonyan.Localization;

public interface IInheritedResourceTypesProvider
{
    [NotNull]
    Type[] GetInheritedResourceTypes();
}
