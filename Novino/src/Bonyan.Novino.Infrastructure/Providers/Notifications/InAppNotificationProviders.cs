using Bonyan.Module.NotificationManagement.Abstractions.Providers;
using Bonyan.Module.NotificationManagement.Abstractions.Types;

namespace Bonyan.Novino.Infrastructure.Providers.Notifications;

public class InAppNotificationProviders : INotificationProvider
{
    public Task SendAsync(string userId, string title, string message, string? link,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }

    public string Key { get; } = "InAppNotificationProviders";
    public NotificationChannel Channel => NotificationChannel.InApp;
}