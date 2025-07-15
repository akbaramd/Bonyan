using System.Threading;
using System.Threading.Tasks;
using Bonyan.Module.NotificationManagement.Abstractions.Types;

namespace Bonyan.Module.NotificationManagement.Abstractions.Providers;

/// <summary>
/// Service responsible for sending notifications through the appropriate providers.
/// </summary>
public interface INotificationSender
{
    /// <summary>
    /// Sends a notification through all active providers for the specified channel.
    /// </summary>
    Task SendAsync(
        NotificationChannel channel,
        string userId,
        string title,
        string message,
        string? link = null,
        CancellationToken cancellationToken = default);
} 