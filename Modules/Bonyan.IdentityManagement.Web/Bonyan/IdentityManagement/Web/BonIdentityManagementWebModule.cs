using Bonyan.Modularity;
using Bonyan.UserManagement.Application;
using Bonyan.UserManagement.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.Web;

public class BonIdentityManagementWebModule<TUser> : BonWebModule where TUser : BonUser
{
    public BonIdentityManagementWebModule()
    {
        DependOn<BonUserManagementApplicationModule<TUser>>();
    }


    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddCookie(IdentityConstants.ApplicationScheme, o =>
            {
                o.LoginPath = new PathString("/Account/Login");
                o.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
                };
            })
            .AddCookie(IdentityConstants.ExternalScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.ExternalScheme;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddCookie(IdentityConstants.TwoFactorRememberMeScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.TwoFactorRememberMeScheme;
                o.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = SecurityStampValidator.ValidateAsync<ITwoFactorSecurityStampValidator>
                };
            })
            .AddCookie(IdentityConstants.TwoFactorUserIdScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.TwoFactorUserIdScheme;
                o.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToReturnUrl = _ => Task.CompletedTask
                };
                o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            }).AddBearerToken();
            
            
        
        return base.OnConfigureAsync(context);
    }
}