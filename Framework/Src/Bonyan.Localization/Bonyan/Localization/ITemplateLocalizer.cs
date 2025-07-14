using Microsoft.Extensions.Localization;

namespace Bonyan.Localization;

public interface ITemplateLocalizer
{
    string Localize(IStringLocalizer localizer, string text);
}
