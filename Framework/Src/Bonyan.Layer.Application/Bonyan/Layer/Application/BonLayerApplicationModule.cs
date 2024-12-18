using Bonyan.AutoMapper;
using Bonyan.Mediators;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UnitOfWork;
using Bonyan.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Layer.Application
{
    public class BonLayerApplicationModule : BonModule
    {
        public BonLayerApplicationModule()
        {
            DependOn(typeof(BonAutoMapperModule),
                typeof(BonUnitOfWorkModule),
                typeof(BonMediatorModule),
                typeof(BonValidationModule));
        }


        public override Task OnConfigureAsync(BonConfigurationContext context)
        {
            context.AddApplication();
            return base.OnConfigureAsync(context);
        }
    }
}
