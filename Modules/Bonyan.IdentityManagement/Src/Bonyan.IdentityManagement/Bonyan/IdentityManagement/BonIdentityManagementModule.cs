using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement;

public class BonIdentityManagementModule : BonWebModule
{
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        return Task.CompletedTask;
    }

    public override Task OnPostConfigureAsync(BonConfigurationContext context)
    {
        var services = context.Services;

        var jwtOptions = context.Services.GetPreConfigureActions<JwtBearerOptions>();
        var cookieOptions = context.Services.GetPreConfigureActions<CookieAuthenticationOptions>();

        var authBuilder = services.AddAuthentication(options =>
        {
            // Default to cookie authentication for web pages
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            // Use JWT Bearer for APIs
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        if (jwtOptions.Any())
            authBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = false; // Set to true in production
                options.SaveToken = true; // Save token for further use
                jwtOptions.Configure(options);
            });

        // Configure cookie authentication
        if (cookieOptions.Any())
            authBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/Account/Login"; // Redirect path for login
                options.LogoutPath = "/Account/Logout"; // Redirect path for logout
                options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect path for access denied

                cookieOptions.Configure(options);
            });
        // Configure JWT authentication

        return base.OnPostConfigureAsync(context);
    }


    public override Task OnApplicationAsync(BonWebApplicationContext context)
    {
        context.Application.UseAuthentication();
        context.Application.UseAuthorization();
        return base.OnApplicationAsync(context);
    }
}