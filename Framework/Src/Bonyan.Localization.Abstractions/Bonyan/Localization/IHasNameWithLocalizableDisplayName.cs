using JetBrains.Annotations;

namespace Bonyan.Localization;

public interface IHasNameWithLocalizableDisplayName
{
    [NotNull]
    public string Name { get; }

    public ILocalizableString? DisplayName { get; }
}
