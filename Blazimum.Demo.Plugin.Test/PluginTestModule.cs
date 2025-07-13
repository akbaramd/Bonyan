using Bonyan.Layer.Domain;
using Bonyan.Messaging;
using Bonyan.Modularity.Abstractions;

namespace Blazimum.Demo.Plugin.Test;


public class PluginTestModule : BonModule
{
    public PluginTestModule()
    {
        DependOn<BonLayerDomainModule>();
        DependOn<BonMessagingModule>();
    }
}