
using Bonyan.AspNetCore.Mvc;
using Bonyan.Modularity;
using Bonyan.AspNetCore.Authentication;
using Bonyan.AspNetCore.Authentication.Cookie;
using Bonyan.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Bonyan.Novino.Web.Menus;
using Bonyan.Novino.Web.Assets;
using Bonyan.IdentityManagement;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Domain.Roles.DomainServices;
using Bonyan.IdentityManagement.Permissions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Bonyan.Novino.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using Bonyan.Novino.Application;
using Bonyan.Novino.Core;
using Bonyan.Novino.Core.Assets;
using Bonyan.Novino.Core.Menus;
using Bonyan.Novino.Domain.Entities;
using Bonyan.TenantManagement.EntityFrameworkCore;
using Bonyan.Novino.Infrastructure;
using Bonyan.Novino.Infrastructure.Data;
using Bonyan.Novino.Module.UserManagement;
using Bonyan.Novino.Web.Models;
using Bonyan.VirtualFileSystem;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Bonyan.Novino.Web;

public class BonNovinoWebModule : BonWebModule
{
    public BonNovinoWebModule()
    {
        DependOn<BonAspNetCoreMvcModule>();
        DependOn<BonAspNetCoreAuthenticationCookieModule>();
        
        DependOn<BonNovinoCoreModule<Domain.Entities.User,Role>>();
        DependOn<BonNovinoInfrastructureModule>();
        DependOn<BonNovinoApplicationModule>();
        
        DependOn<BonNovinoUserManagementModule>();
    }
    
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddEndpointsApiExplorer();
        // Configure menu system
        ConfigureMenuSystem(context);
        
        // Configure asset system
        ConfigureAssetSystem(context);
        
        // Configure authentication
        ConfigureAuthentication(context);
        
        // Configure Razor View Engine for embedded resources
        ConfigureRazorViewEngine(context);

    

        return base.OnConfigureAsync(context);
    }

    private void ConfigureMenuSystem(BonConfigurationContext context)
    {
        // Register sidebar menu locations
        context.Services.AddMenuLocation("sidebar-main", "Sidebar Main", "Main sidebar navigation menu");
        context.Services.AddMenuLocation("sidebar-system", "Sidebar System", "System sidebar navigation menu");
        context.Services.AddMenuLocation("topbar-user", "Topbar User", "Topbar user dropdown menu");
        
        // Register web menu provider
        context.Services.AddMenuProvider<WebMainMenuProvider>();
        
        Configure<BonVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<BonNovinoInfrastructureModule>("Bonyan.Novino.Web", "wwwroot/css");
            
            // Configure embedded views from modules
            options.FileSets.AddEmbedded<BonNovinoUserManagementModule>("Bonyan.Novino.Module.UserManagement", "Areas/UserManagement");
        });

    }
    
    private void ConfigureAssetSystem(BonConfigurationContext context)
    {
        // Configure asset services with fluent API
        context.ConfigureAssets(config =>
        {
            config.AddCommonAssets();
        });
        
        // Register default asset provider
        context.Services.AddAssetProvider<DefaultAssetProvider>();
    }
    
    private void ConfigureAuthentication(BonConfigurationContext context)
    {
        // Configure cookie authentication
        context.Services.Configure<CookieAuthenticationOptions>(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.Cookie.SameSite = SameSiteMode.Lax;
        });
        
        // Register identity services
        context.Services.AddScoped<IBonIdentityUserManager<Domain.Entities.User, Role>, BonIdentityUserManager<Domain.Entities.User, Role>>();
        context.Services.AddScoped<IBonIdentityRoleManager<Role>, BonIdentityRoleManager<Role>>();
        context.Services.AddScoped<IBonPermissionManager<Domain.Entities.User, Role>, BonPermissionManager<Domain.Entities.User, Role>>();
        
        // Register user seeding service
        context.Services.AddScoped<UserSeedingService>();
    }
    
    private void ConfigureRazorViewEngine(BonConfigurationContext context)
    {
        // Configure Razor View Engine to handle embedded resources from modules
        context.Services.Configure<RazorViewEngineOptions>(options =>
        {
            // Add view locations for embedded resources
            options.ViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
            options.ViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
            options.ViewLocationFormats.Add("/Views/{1}/{0}.cshtml");
            options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
        });
    }

    public override Task OnPreApplicationAsync(BonWebApplicationContext context)
    {
        context.Application.UseUnitOfWork();
        return base.OnPreApplicationAsync(context);
    }

    public override async Task OnApplicationAsync(BonWebApplicationContext context)
    {
        context.Application.UseCorrelationId();
        
        try
        {
            // Ensure database is created and migrated
            using var scope = context.Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            // Log the database connection for debugging
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<BonNovinoWebModule>>();
            logger.LogInformation("Connecting to SQL Server database: BonyanNovino");
            
            await dbContext.Database.EnsureCreatedAsync();
            logger.LogInformation("Database created/verified successfully");
            
            // Seed default users
            var userSeedingService = scope.ServiceProvider.GetRequiredService<UserSeedingService>();
            await userSeedingService.SeedDefaultUsersAsync();
        }
        catch (Exception ex)
        {
            var logger = context.Application.Services.GetRequiredService<ILogger<BonNovinoWebModule>>();
            logger.LogError(ex, "Error during database initialization");
            throw;
        }
        
        await base.OnApplicationAsync(context);
    }

    public override Task OnPostApplicationAsync(BonWebApplicationContext context)
    {
        context.Application.UseHttpsRedirection();
        return base.OnPostApplicationAsync(context);
    }
}
