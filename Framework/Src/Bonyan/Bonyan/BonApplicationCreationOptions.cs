using Bonyan.Core;
using Bonyan.Plugins;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan;

public class AbpApplicationCreationOptions
{
    [NotNull]
    public IServiceCollection Services { get; }

    [NotNull]
    public PlugInSourceList PlugInSources { get; }


    public bool SkipConfigureServices { get; set; }

    public string? ApplicationName { get; set; }

    public string? Environment { get; set; }

    public AbpApplicationCreationOptions([NotNull] IServiceCollection services)
    {
        Services = Check.NotNull(services, nameof(services));
        PlugInSources = new PlugInSourceList();
    }
}
