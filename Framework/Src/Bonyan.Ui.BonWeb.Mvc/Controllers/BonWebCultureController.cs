using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Bonyan.Ui.BonWeb.Mvc.Controllers;

/// <summary>
/// Sets the request culture cookie and redirects back. Used by the topbar language switcher.
/// </summary>
[Route("BonWeb/[action]")]
public class BonWebCultureController : Controller
{
    /// <summary>
    /// Set culture cookie and redirect to returnUrl. Supported: en-US, fa-IR.
    /// </summary>
    [HttpGet]
    public IActionResult SetCulture(string culture, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(culture))
        {
            return Redirect(returnUrl ?? "/");
        }

        var normalized = culture.Trim();
        if (normalized != "en-US" && normalized != "fa-IR")
        {
            normalized = "en-US";
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(normalized, normalized)),
            new CookieOptions
            {
                Path = "/",
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                SameSite = SameSiteMode.Lax,
                Secure = Request.IsHttps
            });

        return LocalRedirect(returnUrl ?? "/");
    }
}
