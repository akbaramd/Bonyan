using System.Security.Claims;

namespace Bonyan.Ui.BonWeb.Mvc.Contracts;

/// <summary>
/// Represents a menu item in the BonWeb dashboard.
/// </summary>
public class BonWebMenuItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Order { get; set; } = 0;
    public string? Permission { get; set; }
    public bool RequiresAuthentication { get; set; }
    public List<BonWebMenuItem> Children { get; set; } = new();

    public BonWebMenuItem() { }

    public BonWebMenuItem(string title, string url, string icon = "", int order = 0)
    {
        Title = title;
        Url = url;
        Icon = icon;
        Order = order;
    }

    public bool IsVisible(ClaimsPrincipal? user)
    {
        if (RequiresAuthentication && (user == null || !(user.Identity?.IsAuthenticated ?? false)))
            return false;
        if (!string.IsNullOrEmpty(Permission) && user != null)
        {
            var hasPermission = user.Claims
                .Where(c => c.Type == "permission")
                .Any(c => string.Equals(c.Value, Permission, StringComparison.OrdinalIgnoreCase));
            if (!hasPermission) return false;
        }
        return true;
    }
}
