using Bonyan.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Novino.Module.UserManagement.Areas.UserManagement;

/// <summary>
/// Configuration class for UserManagement module
/// </summary>
public static class UserManagementModuleConfiguration
{
    /// <summary>
    /// Configure UserManagement module services and view locations
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddUserManagementModule(this IServiceCollection services)
    {
        // Add standard view locations for UserManagement module
        services.AddStandardModuleViewLocations("UserManagement");

        // Add custom view locations specific to UserManagement
        services.AddModuleViewLocations("UserManagement",
            // Custom user-specific views
            "/Areas/UserManagement/Pages/User/Details/{0}.cshtml",
            "/Areas/UserManagement/Pages/User/Edit/{0}.cshtml",
            "/Areas/UserManagement/Pages/User/Create/{0}.cshtml",
            
            // Custom partial views
            "/Areas/UserManagement/Pages/User/Partials/{0}.cshtml",
            "/Areas/UserManagement/Pages/User/Shared/{0}.cshtml",
            
            // Custom zone views
            "/Areas/UserManagement/ZoneViews/{0}.cshtml",
            "/Areas/UserManagement/ZoneViews/Tabs/{0}.cshtml",
            "/Areas/UserManagement/ZoneViews/Shared/{0}.cshtml"
        );

        // Add any other UserManagement-specific services here
        // services.AddScoped<IUserService, UserService>();
        // services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
} 