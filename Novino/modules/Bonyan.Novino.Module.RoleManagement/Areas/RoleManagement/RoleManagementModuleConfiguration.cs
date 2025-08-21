using Bonyan.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Novino.Module.RoleManagement.Areas.RoleManagement;

/// <summary>
/// Configuration class for RoleManagement module
/// </summary>
public static class RoleManagementModuleConfiguration
{
    /// <summary>
    /// Configure RoleManagement module services and view locations
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddRoleManagementModule(this IServiceCollection services)
    {
        Console.WriteLine("RoleManagementModuleConfiguration: Configuring RoleManagement module...");
        
        // Add standard view locations for RoleManagement module
        services.AddStandardModuleViewLocations("RoleManagement");

        // Add custom view locations specific to RoleManagement
        services.AddModuleViewLocations("RoleManagement",
            // Custom role-specific views
            "/Areas/RoleManagement/Pages/Role/Details/{0}.cshtml",
            "/Areas/RoleManagement/Pages/Role/Edit/{0}.cshtml",
            "/Areas/RoleManagement/Pages/Role/Create/{0}.cshtml",
            
            // Custom partial views
            "/Areas/RoleManagement/Pages/Role/Partials/{0}.cshtml",
            "/Areas/RoleManagement/Pages/Role/Shared/{0}.cshtml",
            
            // Custom zone views
            "/Areas/RoleManagement/ZoneViews/{0}.cshtml",
            "/Areas/RoleManagement/ZoneViews/Tabs/{0}.cshtml",
            "/Areas/RoleManagement/ZoneViews/Shared/{0}.cshtml",
            
            // User roles integration views
            "/Areas/RoleManagement/Pages/User/Shared/{0}.cshtml",
            "/Areas/RoleManagement/Pages/User/Partials/{0}.cshtml",
            
            // Specific partial for user roles - this is the key fix
            "/Areas/RoleManagement/Pages/Shared/{0}.cshtml",
            "/Areas/RoleManagement/Views/Shared/{0}.cshtml",
            
            // Handle "Shared/" prefix in view names
            "/Areas/RoleManagement/Pages/Shared/Shared/{0}.cshtml",
            "/Areas/RoleManagement/Views/Shared/Shared/{0}.cshtml"
        );

        // Add cross-module view locations for when RoleManagement components are called from other modules
        services.AddCustomViewLocation("/Areas/RoleManagement/Pages/Shared/{0}.cshtml");
        services.AddCustomViewLocation("/Areas/RoleManagement/Views/Shared/{0}.cshtml");
        services.AddCustomViewLocation("/Areas/RoleManagement/ZoneViews/Shared/{0}.cshtml");
        
        // Add cross-module view locations for "Shared/" prefixed view names
        services.AddCustomViewLocation("/Areas/RoleManagement/Pages/Shared/Shared/{0}.cshtml");
        services.AddCustomViewLocation("/Areas/RoleManagement/Views/Shared/Shared/{0}.cshtml");
        services.AddCustomViewLocation("/Areas/RoleManagement/ZoneViews/Shared/Shared/{0}.cshtml");

        Console.WriteLine("RoleManagementModuleConfiguration: RoleManagement module configured successfully!");

        // Add any other RoleManagement-specific services here
        // services.AddScoped<IRoleService, RoleService>();
        // services.AddScoped<IRoleRepository, RoleRepository>();

        return services;
    }
} 