using Microsoft.Extensions.Localization;

namespace Bonyan.AspNetCore.Localization;

public interface ITemplateLocalizer
{
    string Localize(IStringLocalizer localizer, string text);
}
