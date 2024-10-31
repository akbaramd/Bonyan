using Bonyan.DependencyInjection;
using Bonyan.Layer.Domain.Events;
using Bonyan.MultiTenant;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bonyan.Layer.Domain.Services;

public abstract class DomainService : LayServiceProviderConfigurator, IDomainService,ILayServiceProviderConfigurator
{

    protected ILoggerFactory LoggerFactory => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();
    protected ICurrentTenant CurrentTenant => LazyServiceProvider.LazyGetRequiredService<ICurrentTenant>();
    protected IDomainEventDispatcher? DomainEventDispatcher => LazyServiceProvider.LazyGetService<IDomainEventDispatcher>();
    protected ILogger Logger => LazyServiceProvider.LazyGetService<ILogger>(provider => LoggerFactory?.CreateLogger(GetType().FullName!) ?? NullLogger.Instance);
}
