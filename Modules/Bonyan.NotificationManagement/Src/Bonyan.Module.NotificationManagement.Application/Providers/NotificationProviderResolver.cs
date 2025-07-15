using System.Collections.Generic;
using System.Linq;
using Bonyan.Module.NotificationManagement.Abstractions.Options;
using Bonyan.Module.NotificationManagement.Abstractions.Providers;
using Bonyan.Module.NotificationManagement.Abstractions.Types;
using Microsoft.Extensions.Options;

namespace Bonyan.Module.NotificationManagement.Application.Providers;

/// <summary>
/// Resolves active providers for a given channel based on configuration.
/// </summary>
public class NotificationProviderResolver
{
    private readonly IEnumerable<INotificationProvider> _providers;
    private readonly NotificationManagementOptions _options;

    public NotificationProviderResolver(IEnumerable<INotificationProvider> providers, IOptions<NotificationManagementOptions> options)
    {
        _providers = providers;
        _options = options.Value;
    }

    /// <summary>
    /// Gets all active providers for a specific channel.
    /// </summary>
    public IEnumerable<INotificationProvider> GetProvidersForChannel(NotificationChannel channel)
    {
        var configuredProviderKeys = _options.GetProvidersForChannel(channel);
        var providerKeysSet = configuredProviderKeys.ToHashSet();

        return _providers
            .Where(p => p.Channel == channel && providerKeysSet.Contains(p.Key))
            .OrderBy(p => p.Key); // Simple ordering - can be enhanced with priority later
    }

    /// <summary>
    /// Validates that all configured providers are registered.
    /// </summary>
    public bool ValidateConfiguration()
    {
        var registeredKeys = _providers.Select(p => p.Key).ToHashSet();
        return _options.ValidateProviders(registeredKeys);
    }
} 