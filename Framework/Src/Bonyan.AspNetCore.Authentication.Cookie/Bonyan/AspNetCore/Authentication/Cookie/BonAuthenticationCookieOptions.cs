using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Bonyan.AspNetCore.Authentication.Cookie
{
    public class BonAuthenticationCookieOptions
    {
        public bool Enabled { get; set; } = true;
        public string AuthenticationScheme { get; set; } = CookieAuthenticationDefaults.AuthenticationScheme;
        public string CookieName { get; set; } = ".AspNetCore.Cookies";
        public TimeSpan ExpireTimeSpan { get; set; } = TimeSpan.FromDays(14);
        public bool SlidingExpiration { get; set; } = true;
        public string LoginPath { get; set; } = "/Account/Login";
        public string LogoutPath { get; set; } = "/Account/Logout";
        public string AccessDeniedPath { get; set; } = "/Account/AccessDenied";
        public string ReturnUrlParameter { get; set; } = "returnUrl";
        public bool HttpOnly { get; set; } = true;
        public CookieSecurePolicy SecurePolicy { get; set; } = CookieSecurePolicy.SameAsRequest;
        public SameSiteMode SameSite { get; set; } = SameSiteMode.Lax;
        public bool RequireHttps { get; set; } = false;
        public string Domain { get; set; } = string.Empty;
        public string Path { get; set; } = "/";
        public TimeSpan? SessionTimeout { get; set; }
        public bool ValidateInterval { get; set; } = true;
        public TimeSpan ValidationInterval { get; set; } = TimeSpan.FromMinutes(30);
    }
} 