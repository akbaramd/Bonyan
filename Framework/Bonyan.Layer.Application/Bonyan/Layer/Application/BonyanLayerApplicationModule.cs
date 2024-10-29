using Bonyan.AutoMapper;
using Bonyan.Modularity.Attributes;
using Module = Bonyan.Modularity.Abstractions.Module;

namespace Bonyan.Layer.Application
{
    [DependOn(typeof(BonyanAutoMapperModule),typeof(BonyanUnitOfWorkModule))]
    public class BonyanLayerApplicationModule : Module
    {
        
    }
}
