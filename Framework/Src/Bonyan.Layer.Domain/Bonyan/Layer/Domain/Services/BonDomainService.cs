using Bonyan.DependencyInjection;
using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.Layer.Domain.DomainService;
using Bonyan.MultiTenant;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bonyan.Layer.Domain.Services;

public abstract class BonDomainService : BonLayServiceProviderConfigurator, IBonDomainService
{

    protected ILoggerFactory LoggerFactory => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();
    protected IBonCurrentTenant BonCurrentTenant => LazyServiceProvider.LazyGetRequiredService<IBonCurrentTenant>();
    protected IBonDomainEventDispatcher? DomainEventDispatcher => LazyServiceProvider.LazyGetService<IBonDomainEventDispatcher>();
    protected ILogger Logger => LazyServiceProvider.LazyGetService<ILogger>(provider => LoggerFactory?.CreateLogger(GetType().FullName!) ?? NullLogger.Instance); }
