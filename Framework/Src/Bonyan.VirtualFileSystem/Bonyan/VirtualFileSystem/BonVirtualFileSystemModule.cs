using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.VirtualFileSystem;

public class BonVirtualFileSystemModule : BonModule
{
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddSingleton<VirtualFileProvider>();
        context.Services.AddSingleton<IVirtualFileProvider,VirtualFileProvider>();

        context.Services.AddSingleton<DynamicFileProvider>();
        context.Services.AddSingleton<IDynamicFileProvider,DynamicFileProvider>();
        return base.OnConfigureAsync(context);
    }
}
