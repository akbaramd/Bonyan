using Bonyan.AutoMapper;
using Bonyan.Modularity.Abstractions;
using Bonyan.UnitOfWork;
using Bonyan.Validation;

namespace Bonyan.Layer.Application
{
    public class BonLayerApplicationModule : BonModule
    {
        public BonLayerApplicationModule()
        {
            DependOn(typeof(BonAutoMapperModule),
                typeof(BonUnitOfWorkModule),
                typeof(BonyanValidationModule));
        }        
    }
}
