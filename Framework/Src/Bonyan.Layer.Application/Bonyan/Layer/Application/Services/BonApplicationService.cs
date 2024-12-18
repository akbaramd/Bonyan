﻿using AutoMapper;
using Bonyan.DependencyInjection;
using Bonyan.Mediators;
using Bonyan.MultiTenant;
using Bonyan.UnitOfWork;
using Bonyan.User;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bonyan.Layer.Application.Services;

public class BonApplicationService :  BonLayServiceProviderConfigurator, IBonApplicationService,IBonUnitOfWorkEnabled
{
    public IBonCurrentUser BonCurrentUser => LazyServiceProvider.LazyGetRequiredService<IBonCurrentUser>();
    public IBonCurrentTenant BonCurrentTenant => LazyServiceProvider.LazyGetRequiredService<IBonCurrentTenant>();
    public IMapper Mapper => LazyServiceProvider.LazyGetRequiredService<IMapper>();
    public IBonMediator BonMessageBus => LazyServiceProvider.LazyGetRequiredService<IBonMediator>();

    protected IBonUnitOfWorkManager UnitOfWorkManager =>
        LazyServiceProvider.LazyGetRequiredService<IBonUnitOfWorkManager>();

    protected ILoggerFactory LoggerFactory => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();

    protected ILogger Logger => LazyServiceProvider.LazyGetService<ILogger>(provider =>
        LoggerFactory?.CreateLogger(GetType().FullName!) ?? NullLogger.Instance);

    protected IBonUnitOfWork? CurrentUnitOfWork => UnitOfWorkManager.Current;
}