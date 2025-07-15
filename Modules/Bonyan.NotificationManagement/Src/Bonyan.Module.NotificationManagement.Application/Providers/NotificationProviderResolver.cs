using System;
using System.Collections.Generic;
using System.Linq;
using Bonyan.Module.NotificationManagement.Abstractions.Providers;
using Bonyan.Module.NotificationManagement.Abstractions.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bonyan.Module.NotificationManagement.Application.Providers;

/// <summary>
/// Resolves active providers for a given channel based on configuration.
/// </summary>
public class NotificationProviderResolver
{
    private readonly IServiceProvider _providers;

    public NotificationProviderResolver(IServiceProvider providers)
    {
        _providers = providers;
    }

    /// <summary>
    /// Gets all active providers for a specific channel.
    /// </summary>
    public IEnumerable<INotificationProvider> GetProvidersForChannel(NotificationChannel channel)
    {


        return _providers.GetServices<INotificationProvider>()
            .Where(p => p.Channel == channel)
            .OrderBy(p => p.Key); // Simple ordering - can be enhanced with priority later
    }

} 