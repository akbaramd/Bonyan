using Bonyan.Modularity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.AspNetCore.Authentication.Cookie
{
    public static class BonAuthenticationCookieExtensions
    {
        /// <summary>
        /// Configures cookie authentication options
        /// </summary>
        /// <param name="context">The configuration context</param>
        /// <param name="configureOptions">Action to configure cookie options</param>
        public static void ConfigureCookieAuthentication(this BonConfigurationContext context, Action<BonAuthenticationCookieOptions> configureOptions)
        {
            context.Services.PreConfigure<BonAuthenticationCookieOptions>(configureOptions);
        }

        /// <summary>
        /// Configures cookie authentication options with basic settings
        /// </summary>
        /// <param name="context">The configuration context</param>
        /// <param name="cookieName">The cookie name</param>
        /// <param name="expireTimeSpan">The cookie expiration time</param>
        /// <param name="loginPath">The login path</param>
        /// <param name="logoutPath">The logout path</param>
        /// <param name="enabled">Whether cookie authentication is enabled</param>
        public static void ConfigureCookieAuthentication(this BonConfigurationContext context, 
            string cookieName = ".AspNetCore.Cookies", 
            TimeSpan? expireTimeSpan = null, 
            string loginPath = "/Account/Login", 
            string logoutPath = "/Account/Logout", 
            bool enabled = true)
        {
            context.Services.PreConfigure<BonAuthenticationCookieOptions>(options =>
            {
                options.Enabled = enabled;
                options.CookieName = cookieName;
                options.ExpireTimeSpan = expireTimeSpan ?? TimeSpan.FromDays(14);
                options.LoginPath = loginPath;
                options.LogoutPath = logoutPath;
            });
        }

        /// <summary>
        /// Configures cookie authentication for production environment
        /// </summary>
        /// <param name="context">The configuration context</param>
        /// <param name="domain">The cookie domain</param>
        /// <param name="requireHttps">Whether to require HTTPS</param>
        public static void ConfigureCookieAuthenticationForProduction(this BonConfigurationContext context, 
            string domain = "", 
            bool requireHttps = true)
        {
            context.Services.PreConfigure<BonAuthenticationCookieOptions>(options =>
            {
                options.RequireHttps = requireHttps;
                options.SecurePolicy = requireHttps ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
                options.SameSite = SameSiteMode.Strict;
                options.HttpOnly = true;
                
                if (!string.IsNullOrEmpty(domain))
                {
                    options.Domain = domain;
                }
            });
        }

        /// <summary>
        /// Configures cookie authentication for development environment
        /// </summary>
        /// <param name="context">The configuration context</param>
        public static void ConfigureCookieAuthenticationForDevelopment(this BonConfigurationContext context)
        {
            context.Services.PreConfigure<BonAuthenticationCookieOptions>(options =>
            {
                options.RequireHttps = false;
                options.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.SameSite = SameSiteMode.Lax;
                options.ValidationInterval = TimeSpan.FromMinutes(1); // Shorter validation for development
            });
        }
    }
} 