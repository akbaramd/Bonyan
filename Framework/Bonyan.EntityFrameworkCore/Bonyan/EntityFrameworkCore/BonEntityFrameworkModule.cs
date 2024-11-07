using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.EntityFrameworkCore
{
    public class BonEntityFrameworkModule : BonModule
    {
        public BonEntityFrameworkModule()
        {
            DependOn<BonUnitOfWorkModule>();
        }

        public override Task OnConfigureAsync(BonConfigurationContext context)
        {
            context.Services.AddTransient(typeof(IBonDbContextProvider<>), typeof(BonUnitOfWorkBonDbContextProvider<>));
            return base.OnConfigureAsync(context);
        }
    }
}