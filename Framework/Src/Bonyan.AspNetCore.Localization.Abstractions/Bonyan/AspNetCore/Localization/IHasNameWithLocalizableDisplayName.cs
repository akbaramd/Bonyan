using JetBrains.Annotations;

namespace Bonyan.AspNetCore.Localization;

public interface IHasNameWithLocalizableDisplayName
{
    [NotNull]
    public string Name { get; }

    public ILocalizableString? DisplayName { get; }
}
