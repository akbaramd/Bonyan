using Bonyan.AspNetCore.Authentication.Cookie;
using Bonyan.AspNetCore.Mvc;
using Bonyan.IdentityManagement.BonWeb.Mvc;
using Bonyan.Modularity;
using Bonyan.Ui.BonWeb.Mvc;
using Bonyan.UnitOfWork;

using BonyanTemplate.Application;
using BonyanTemplate.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BonyanTemplate.Mvc;

/// <summary>
/// MVC host module for the Bonyan Template. Uses Application and Infrastructure,
/// configures cookie authentication and authorization for browser-based login.
/// </summary>
public class BonyanTemplateMvcModule : BonWebModule
{
    public BonyanTemplateMvcModule()
    {
        DependOn<BonyanTemplateApplicationModule>();
        DependOn<BonyanTemplateInfrastructureModule>();
        DependOn<BonWebMvcModule>();
        DependOn<BonIdentityManagementBonWebMvcModule>();
        DependOn<BonAspNetCoreMvcModule>();
        DependOn<BonUnitOfWorkModule>();
        DependOn<BonAspNetCoreAuthenticationCookieModule>();
    }

    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        context.ConfigureCookieAuthentication(options =>
        {
            options.Enabled = true;
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ReturnUrlParameter = "returnUrl";
            options.ExpireTimeSpan = TimeSpan.FromDays(14);
            options.SlidingExpiration = true;
        });

        context.Services.PreConfigure<AuthenticationOptions>(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        });

        return base.OnConfigureAsync(context, cancellationToken);
    }

    public override ValueTask OnApplicationAsync(BonWebApplicationContext context, CancellationToken cancellationToken = default)
    {
        context.Application.UseCorrelationId();
        return base.OnApplicationAsync(context, cancellationToken);
    }

    public override ValueTask OnPostApplicationAsync(BonWebApplicationContext context, CancellationToken cancellationToken = default)
    {
        context.Application.UseHttpsRedirection();
        return base.OnPostApplicationAsync(context, cancellationToken);
    }
}
