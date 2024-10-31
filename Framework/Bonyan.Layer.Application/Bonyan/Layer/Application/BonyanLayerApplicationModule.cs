using Bonyan.AutoMapper;
using Bonyan.Modularity;
using Bonyan.UnitOfWork;
using Bonyan.Validation;
using Module = Bonyan.Modularity.Abstractions.Module;

namespace Bonyan.Layer.Application
{
    public class BonyanLayerApplicationModule : Module
    {
        public BonyanLayerApplicationModule()
        {
            DependOn(typeof(BonyanAutoMapperModule),
                typeof(BonyanUnitOfWorkModule),
                typeof(BonyanValidationModule));
        }        
    }
}
