using Bonyan.AspNetCore;
using Bonyan.Layer.Application;
using Bonyan.Layer.Domain;
using Bonyan.Modularity;

namespace Novin.AspNetCore.Novin.AspNetCore;

public class NovinModule : BonWebModule
{
    public NovinModule()
    {
        DependOn<BonAspNetCoreModule>();
        DependOn<BonLayerApplicationModule>();
        DependOn<BonLayerDomainModule>();
    }
}