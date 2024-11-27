﻿using Bonyan.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionPreConfigureExtensions
{
    public static IServiceCollection PreConfigure<TOptions>(this IServiceCollection services, Action<TOptions> optionsAction)
    {
        services.GetPreConfigureActions<TOptions>().Add(optionsAction);
        return services;
    }

    public static TOptions ExecutePreConfiguredActions<TOptions>(this IServiceCollection services)
        where TOptions : new()
    {
        return services.ExecutePreConfiguredActions(new TOptions());
    }

    public static TOptions ExecutePreConfiguredActions<TOptions>(this IServiceCollection services, TOptions options)
    {
        services.GetPreConfigureActions<TOptions>().Configure(options);
        return options;
    }

    public static BonPreConfigureActionList<TOptions> GetPreConfigureActions<TOptions>(this IServiceCollection services)
    {
        var actionList = services.GetSingletonInstanceOrNull<IBonObjectAccessor<BonPreConfigureActionList<TOptions>>>()?.Value;
        if (actionList == null)
        {
            actionList = new BonPreConfigureActionList<TOptions>();
            services.AddObjectAccessor(actionList);
        }

        return actionList;
    }
}