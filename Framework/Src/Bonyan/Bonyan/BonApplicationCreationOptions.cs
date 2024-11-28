using Bonyan.Core;
using Bonyan.Plugins;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan;

public class BonApplicationCreationOptions
{
    [NotNull]
    public IServiceCollection Services { get; }

    [NotNull]
    public PlugInSourceList PlugInSources { get; }


    public bool SkipConfigureServices { get; set; }


    public string? Environment { get; set; }

    public BonApplicationCreationOptions([NotNull] IServiceCollection services)
    {
        Services = Check.NotNull(services, nameof(services));
        PlugInSources = new PlugInSourceList();
    }
}
