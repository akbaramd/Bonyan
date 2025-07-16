using Bonyan.IdentityManagement.Permissions;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Novino.Core.Menus;
using Bonyan.Novino.Infrastructure;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Permissions;
using Bonyan.Novino.Module.UserManagement.Menus;
using Bonyan.VirtualFileSystem;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Bonyan.Novino.Module.UserManagement;

public class BonNovinoUserManagementModule : BonModule
{
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddTransient<IBonPermissionDefinitionProvider, UserManagementPermissionProvider>();
        
        context.Services.AddMenuProvider<UserManagementMenuProvider>();
        
                Configure<BonEndpointRouterOptions>(options =>
        {
            options.EndpointConfigureActions.Add(endpointContext =>
            {
                // Configure area routing
                endpointContext.Endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                
                // 2️⃣ Area route only after default
                endpointContext.Endpoints.MapControllerRoute(
                    name: "UserManagement",
                    pattern: "UserManagement/{controller=User}/{action=Index}/{id?}",
                    defaults: new { area = "UserManagement" }
                );
            });
        });

        Configure<BonVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<BonNovinoUserManagementModule>("Bonyan.Novino.Module.UserManagement", "Areas/UserManagement");
        });


        return base.OnConfigureAsync(context);
    }

    public override Task OnInitializeAsync(BonInitializedContext context)
    {

        return base.OnInitializeAsync(context);
    }

  

} 