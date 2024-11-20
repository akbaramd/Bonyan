using System.Text;
using Bonyan.IdentityManagement;
using Bonyan.Modularity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Bonyan.AspNetCore.Authentication;

public class BonAspNetCoreAuthenticationModule : BonWebModule
{
    public BonAspNetCoreAuthenticationModule()
    {
        DependOn<BonAspNetCoreModule>();
    }
    
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        return Task.CompletedTask;
    }

    public override Task OnPostConfigureAsync(BonConfigurationContext context)
    {
        var jwtOptions = context.GetOption<BonJwtOptions>();
        var cookieOptions = context.GetOption<BonCookieOptions>();

        var services = context.Services;

        var authBuilder = services.AddAuthentication();

        // Configure JWT authentication if enabled
        if (jwtOptions?.Enabled == true && !string.IsNullOrWhiteSpace(jwtOptions.SecretKey))
        {
            authBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = jwtOptions.RequireHttpsMetadata;
                options.SaveToken = jwtOptions.SaveToken;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = !string.IsNullOrEmpty(jwtOptions.Issuer),
                    ValidateAudience = !string.IsNullOrEmpty(jwtOptions.Audience),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience
                };
            });
        }

        // Configure cookie authentication
        if (cookieOptions != null)
        {
            authBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = cookieOptions.LoginPath;
                options.LogoutPath = cookieOptions.LogoutPath;
                options.AccessDeniedPath = cookieOptions.AccessDeniedPath;

                options.SlidingExpiration = cookieOptions.SlidingExpiration;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(cookieOptions.ExpirationInMinutes);
                options.Cookie.Name = cookieOptions.CookieName;
                options.Cookie.SecurePolicy = cookieOptions.CookieSecure
                    ? CookieSecurePolicy.Always
                    : CookieSecurePolicy.None;
            });
        }

        return base.OnPostConfigureAsync(context);
    }

    public override Task OnApplicationAsync(BonWebApplicationContext context)
    {
        context.Application.UseAuthentication();
        context.Application.UseAuthorization();
        return base.OnApplicationAsync(context);
    }
}