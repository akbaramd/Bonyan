using Bonyan.IdentityManagement.Permissions;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Novino.Core.Menus;
using Bonyan.Novino.Infrastructure;
using Bonyan.Novino.Module.RoleManagement.Areas.RoleManagement;
using Bonyan.Novino.Module.RoleManagement.Areas.RoleManagement.Permissions;
using Bonyan.VirtualFileSystem;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Bonyan.Novino.Module.RoleManagement;

public class BonNovinoRoleManagementModule : BonModule
{
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        Console.WriteLine("BonNovinoRoleManagementModule: Starting configuration...");
        
        context.Services.AddRoleManagementModule();
        Console.WriteLine("BonNovinoRoleManagementModule: RoleManagement module added to services");
        
        // Register zone components from this assembly
        context.Services.AddZoneComponentsFrom(GetType().Assembly);
        Console.WriteLine("BonNovinoRoleManagementModule: Zone components registered from assembly");
        
        context.Services.AddSingleton<IBonPermissionDefinitionProvider, RoleManagementPermissionProvider>();
        Console.WriteLine("BonNovinoRoleManagementModule: Permission provider registered");
        // Register nested view handlers

        
                Configure<BonEndpointRouterOptions>(options =>
        {
            options.EndpointConfigureActions.Add(endpointContext =>
            {
    
                
                // 2️⃣ Area route only after default
                endpointContext.Endpoints.MapControllerRoute(
                    name: "RoleManagement",
                    pattern: "RoleManagement/{controller=User}/{action=Index}/{id?}",
                    defaults: new { area = "RoleManagement" }
                );
            });
        });

        Configure<BonVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<BonNovinoRoleManagementModule>("Bonyan.Novino.Module.RoleManagement",
                "Areas/RoleManagement");
        });


        return base.OnConfigureAsync(context);
    }

    public override Task OnInitializeAsync(BonInitializedContext context)
    {

        return base.OnInitializeAsync(context);
    }

  

} 