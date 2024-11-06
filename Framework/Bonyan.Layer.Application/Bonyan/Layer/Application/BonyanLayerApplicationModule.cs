using Bonyan.AutoMapper;
using Bonyan.Modularity.Abstractions;
using Bonyan.UnitOfWork;
using Bonyan.Validation;

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
