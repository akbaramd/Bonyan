using System.Collections.Generic;
using System.Linq;
using Bonyan.Module.NotificationManagement.Abstractions.Types;

namespace Bonyan.Module.NotificationManagement.Abstractions.Options;

/// <summary>
/// Options used to configure notification providers and behaviour via configuration (appsettings, etc.).
/// </summary>
public class NotificationManagementOptions
{
    /// <summary>
    /// List of active providers for each channel.
    /// </summary>
    public Dictionary<NotificationChannel, List<string>> Providers { get; set; } = new();

    /// <summary>
    /// Validates that all provider keys exist in the registered providers.
    /// </summary>
    public bool ValidateProviders(IEnumerable<string> registeredProviderKeys)
    {
        var registeredKeys = registeredProviderKeys.ToHashSet();
        
        foreach (var channelProviders in Providers.Values)
        {
            foreach (var providerKey in channelProviders)
            {
                if (!registeredKeys.Contains(providerKey))
                {
                    return false;
                }
            }
        }
        
        return true;
    }

    /// <summary>
    /// Gets all provider keys for a specific channel.
    /// </summary>
    public IEnumerable<string> GetProvidersForChannel(NotificationChannel channel)
    {
        return Providers.TryGetValue(channel, out var providers) ? providers : Enumerable.Empty<string>();
    }
} 