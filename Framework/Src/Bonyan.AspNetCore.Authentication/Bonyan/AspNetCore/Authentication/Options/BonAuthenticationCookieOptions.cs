namespace Bonyan.AspNetCore.Authentication.Options;

public class BonAuthenticationCookieOptions
{
    public string LoginPath { get; set; } = "/Account/Login";
    public string AccessDeniedPath { get; set; } = "/Account/AccessDenied";
    public string LogoutPath { get; set; } = "/Account/Logout";
    public bool SlidingExpiration { get; set; } = true;
    public int ExpirationInMinutes { get; set; } = 60;
    public string CookieName { get; set; } = ".Bon.Authentication.Cookie";
    public bool CookieSecure { get; set; } = false;
}