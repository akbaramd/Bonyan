using AutoMapper;
using Bonyan.DependencyInjection;
using Bonyan.MultiTenant;
using Bonyan.UnitOfWork;
using Bonyan.User;

namespace Bonyan.Layer.Application.Services;

public class ApplicationService : LayServiceProviderConfigurator, IApplicationService
{
    public ICurrentUser CurrentUser => LazyServiceProvider.LazyGetRequiredService<ICurrentUser>();
    public ICurrentTenant CurrentTenant => LazyServiceProvider.LazyGetRequiredService<ICurrentTenant>();
    public IMapper Mapper => LazyServiceProvider.LazyGetRequiredService<IMapper>();
    protected IUnitOfWorkManager UnitOfWorkManager => LazyServiceProvider.LazyGetRequiredService<IUnitOfWorkManager>();
    protected IUnitOfWork? CurrentUnitOfWork => UnitOfWorkManager?.Current;
}