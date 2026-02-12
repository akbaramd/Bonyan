using System.Reflection;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bonyan.VirtualFileSystem;

public class BonVirtualFileSystemModule : BonModule
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        // Register Assembly so design-time or any code that resolves BonEmbeddedFileProvider (which requires Assembly) can get it.
        context.Services.TryAddSingleton(typeof(Assembly), _ =>
            Assembly.GetEntryAssembly() ?? typeof(BonVirtualFileSystemModule).Assembly);

        context.Services.AddSingleton<VirtualFileProvider>();
        context.Services.AddSingleton<IVirtualFileProvider, VirtualFileProvider>();

        context.Services.AddSingleton<DynamicFileProvider>();
        context.Services.AddSingleton<IDynamicFileProvider, DynamicFileProvider>();
        return base.OnConfigureAsync(context, cancellationToken);
    }
}
