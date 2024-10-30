using Bonyan.DependencyInjection;
using Bonyan.MultiTenant;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bonyan.Layer.Domain.Services;

public abstract class DomainService : IDomainService
{
    public IBonyanLazyServiceProvider LazyServiceProvider { get; set; } = default!;

    protected ILoggerFactory LoggerFactory => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();

    protected ICurrentTenant CurrentTenant => LazyServiceProvider.LazyGetRequiredService<ICurrentTenant>();


    protected ILogger Logger => LazyServiceProvider.LazyGetService<ILogger>(provider => LoggerFactory?.CreateLogger(GetType().FullName!) ?? NullLogger.Instance);
}
