using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.Module.NotificationManagement.Abstractions.Providers;
using Bonyan.Module.NotificationManagement.Abstractions.Types;
using Bonyan.Module.NotificationManagement.Domain.Notifications;
using Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;

namespace Bonyan.Module.NotificationManagement.Infrastructure.Providers;

/// <summary>
/// Implementation of INotificationSender that routes notifications through configured providers.
/// </summary>
public class NotificationSender : INotificationSender
{
    private readonly NotificationProviderResolver _resolver;
    private readonly IBonNotificationRepository _repository;

    public NotificationSender(NotificationProviderResolver resolver, IBonNotificationRepository repository)
    {
        _resolver = resolver;
        _repository = repository;
    }

    public async Task SendAsync(
        NotificationChannel channel,
        string userId,
        string title,
        string message,
        string? link = null,
        CancellationToken cancellationToken = default)
    {
        // Get all active providers for this channel
        var providers = _resolver.GetProvidersForChannel(channel).ToList();

        if (!providers.Any())
        {
            throw new InvalidOperationException($"No active providers found for channel: {channel}");
        }

        // Send through all providers
        var tasks = providers.Select(provider => 
            SendThroughProvider(provider, userId, title, message, link, cancellationToken));

        await Task.WhenAll(tasks);

        // Store notification in domain
        var notification = new BonNotification(userId, title, message, link);
        await _repository.AddAsync(notification, autoSave: true);
    }

    private async Task SendThroughProvider(
        INotificationProvider provider,
        string userId,
        string title,
        string message,
        string? link,
        CancellationToken cancellationToken)
    {
        try
        {
            await provider.SendAsync(userId, title, message, link, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log error but don't fail the entire operation
            // In a real implementation, you might want to store failed attempts
            Console.WriteLine($"Failed to send notification through provider {provider.Key}: {ex.Message}");
        }
    }
} 