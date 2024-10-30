using Bonyan.AutoMapper;
using Bonyan.Modularity;
using Bonyan.UnitOfWork;
using Bonyan.Validation;
using Module = Bonyan.Modularity.Abstractions.Module;

namespace Bonyan.Layer.Application
{
    [DependOn(typeof(BonyanAutoMapperModule),
        typeof(BonyanUnitOfWorkModule),
        typeof(BonyanValidationModule)
        )]
    public class BonyanLayerApplicationModule : Module
    {
        
    }
}
