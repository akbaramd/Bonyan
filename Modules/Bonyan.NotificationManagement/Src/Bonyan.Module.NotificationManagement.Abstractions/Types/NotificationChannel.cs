namespace Bonyan.Module.NotificationManagement.Abstractions.Types;

/// <summary>
/// Global channels supported by the notification module.
/// Keep in Abstractions so every layer (Domain/Application/Infra/UI) can reference it without pulling the Domain package.
/// </summary>
public enum NotificationChannel
{
    Email   = 1,
    Sms     = 2,
    Push    = 3,
    InApp   = 4,
    Webhook = 5,
    Chat    = 6 // Generic chat (Slack, Teams, Discord, ...)
} 