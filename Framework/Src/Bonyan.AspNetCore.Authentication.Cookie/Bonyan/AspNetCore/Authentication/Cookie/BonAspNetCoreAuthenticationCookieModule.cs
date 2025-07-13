using Bonyan.Modularity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Bonyan.AspNetCore.Authentication;

namespace Bonyan.AspNetCore.Authentication.Cookie
{
    public class BonAspNetCoreAuthenticationCookieModule : BonWebModule
    {
        public BonAspNetCoreAuthenticationCookieModule()
        {
            DependOn<BonAspnetCoreAuthenticationModule>();
        }

        public override Task OnPreConfigureAsync(BonConfigurationContext context)
        {
            PreConfigure<AuthenticationBuilder>(c =>
            {
                var cookieOptions = new BonAuthenticationCookieOptions();
                context.Services.ExecutePreConfiguredActions(cookieOptions);
                context.Services.AddSingleton(cookieOptions);
                
                if (cookieOptions.Enabled)
                {
                    c.AddCookie(cookieOptions.AuthenticationScheme, options =>
                    {
                        options.Cookie.Name = cookieOptions.CookieName;
                        options.ExpireTimeSpan = cookieOptions.ExpireTimeSpan;
                        options.SlidingExpiration = cookieOptions.SlidingExpiration;
                        options.LoginPath = cookieOptions.LoginPath;
                        options.LogoutPath = cookieOptions.LogoutPath;
                        options.AccessDeniedPath = cookieOptions.AccessDeniedPath;
                        options.ReturnUrlParameter = cookieOptions.ReturnUrlParameter;
                        options.Cookie.HttpOnly = cookieOptions.HttpOnly ;
                        options.Cookie.SecurePolicy = cookieOptions.SecurePolicy;
                        options.Cookie.SameSite = cookieOptions.SameSite;
                        
                        if (!string.IsNullOrEmpty(cookieOptions.Domain))
                        {
                            options.Cookie.Domain = cookieOptions.Domain;
                        }
                        
                        if (!string.IsNullOrEmpty(cookieOptions.Path))
                        {
                            options.Cookie.Path = cookieOptions.Path;
                        }

                        // Configure session timeout if specified
                        if (cookieOptions.SessionTimeout.HasValue)
                        {
                            options.ExpireTimeSpan = cookieOptions.SessionTimeout.Value;
                        }

                        // Configure validation interval
                        if (cookieOptions.ValidateInterval)
                        {
                            options.Events = new CookieAuthenticationEvents
                            {
                                OnValidatePrincipal = context =>
                                {
                                    return ValidatePrincipalAsync(context, cookieOptions);
                                }
                            };
                        }
                    });
                }
            });

            return base.OnPreConfigureAsync(context);
        }

        public override Task OnPostConfigureAsync(BonConfigurationContext context)
        {
            return base.OnPostConfigureAsync(context);
        }

        public override Task OnApplicationAsync(BonWebApplicationContext context)
        {
            return base.OnApplicationAsync(context);
        }

        private async Task ValidatePrincipalAsync(CookieValidatePrincipalContext context, BonAuthenticationCookieOptions cookieOptions)
        {
            var utcNow = DateTimeOffset.UtcNow;
            var issuedUtc = context.Properties.IssuedUtc;
            
            if (issuedUtc != null)
            {
                var timeElapsed = utcNow.Subtract(issuedUtc.Value);
                
                if (timeElapsed > cookieOptions.ValidationInterval)
                {
                    // Renew the cookie
                    context.ShouldRenew = true;
                }
            }
            
            await Task.CompletedTask;
        }
    }
} 