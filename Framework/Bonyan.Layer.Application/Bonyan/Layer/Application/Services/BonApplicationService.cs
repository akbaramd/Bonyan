using AutoMapper;
using Bonyan.DependencyInjection;
using Bonyan.MultiTenant;
using Bonyan.UnitOfWork;
using Bonyan.User;

namespace Bonyan.Layer.Application.Services;

public class BonApplicationService : BonLayServiceProviderConfigurator, IBonApplicationService
{
    public IBonCurrentUser BonCurrentUser => LazyServiceProvider.LazyGetRequiredService<IBonCurrentUser>();
    public IBonCurrentTenant BonCurrentTenant => LazyServiceProvider.LazyGetRequiredService<IBonCurrentTenant>();
    public IMapper Mapper => LazyServiceProvider.LazyGetRequiredService<IMapper>();
    protected IBonUnitOfWorkManager BonUnitOfWorkManager => LazyServiceProvider.LazyGetRequiredService<IBonUnitOfWorkManager>();
    protected IBonUnitOfWork? CurrentUnitOfWork => BonUnitOfWorkManager.Current;
}