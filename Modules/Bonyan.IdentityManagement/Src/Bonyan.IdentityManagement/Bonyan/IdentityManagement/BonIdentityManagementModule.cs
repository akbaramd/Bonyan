using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement;

public class BonIdentityManagementModule : BonModule
{
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddAuthentication(c =>
        {
            c.DefaultScheme = "";
        });
        
        return base.OnConfigureAsync(context);
    }
}