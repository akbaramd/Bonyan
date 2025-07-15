using System.Threading;
using System.Threading.Tasks;
using Bonyan.Module.NotificationManagement.Abstractions.Types;

namespace Bonyan.Module.NotificationManagement.Abstractions.Providers;

public interface INotificationProvider
{
    /// <summary>
    /// Unique key for the provider (e.g., "sendgrid", "twilio").
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Supported channel.
    /// </summary>
    NotificationChannel Channel { get; }

    Task SendAsync(string userId, string title, string message, string? link, CancellationToken cancellationToken = default);
} 