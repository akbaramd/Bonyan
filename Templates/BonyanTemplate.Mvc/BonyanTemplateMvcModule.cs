using Bonyan.AspNetCore.Authentication.Cookie;
using Bonyan.AspNetCore.Mvc;
using Bonyan.IdentityManagement.BonWeb.Mvc;
using Bonyan.Modularity;
using Bonyan.Ui.BonWeb.Mvc;
using Bonyan.UnitOfWork;
using Bonyan.VirtualFileSystem;

using BonyanTemplate.Application;
using BonyanTemplate.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

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

    public override ValueTask OnPreConfigureAsync(BonPreConfigurationContext context, CancellationToken cancellationToken = default)
    {
#if DEBUG
        // In Debug: view changes in this project (Views/*.cshtml) are picked up on refresh without rebuild.
        context.PreConfigure<IMvcBuilder>(b => b.AddRazorRuntimeCompilation());
#endif
        return base.OnPreConfigureAsync(context, cancellationToken);
    }

    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        // Serve Localization JSON from output directory so module localization (e.g. IdentityManagement) is found when embedded provider path differs.
        context.Services.Configure<BonVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddPhysical(AppContext.BaseDirectory);
        });

        context.ConfigureCookieAuthentication(options =>
        {
            options.Enabled = true;
            options.LoginPath = "/IdentityManagement/Account/Login";
            options.LogoutPath = "/IdentityManagement/Account/Logout";
            options.AccessDeniedPath = "/IdentityManagement/Account/AccessDenied";
            options.ReturnUrlParameter = "returnUrl";
            options.ExpireTimeSpan = TimeSpan.FromDays(14);
            options.SlidingExpiration = true;
        });

        context.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("AuthEndpoints", config =>
            {
                config.PermitLimit = 10;
                config.Window = TimeSpan.FromMinutes(1);
                config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                config.QueueLimit = 0;
            });
            options.OnRejected = async (context, _) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.Headers.RetryAfter = "30";
                await context.HttpContext.Response.WriteAsJsonAsync(new { error = "TooManyAttempts", retryAfterSeconds = 30 });
            };
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
        context.Application.UseRateLimiter();
        return base.OnApplicationAsync(context, cancellationToken);
    }

    public override ValueTask OnPostApplicationAsync(BonWebApplicationContext context, CancellationToken cancellationToken = default)
    {
        context.Application.UseHttpsRedirection();
        return base.OnPostApplicationAsync(context, cancellationToken);
    }
}
