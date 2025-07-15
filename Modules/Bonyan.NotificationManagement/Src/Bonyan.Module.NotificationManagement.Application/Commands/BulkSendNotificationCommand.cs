using System.Collections.Generic;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Abstractions.Providers;
using Bonyan.Module.NotificationManagement.Abstractions.Types;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public class BulkSendNotificationCommand : IBonCommand
{
    public List<string> UserIds { get; }
    public string Title { get; }
    public string Message { get; }
    public string? Link { get; }
    public string? Purpose { get; }
    public string? Context { get; }
    public NotificationChannel Channel { get; }

    public BulkSendNotificationCommand(
        List<string> userIds, 
        string title, 
        string message, 
        NotificationChannel channel, 
        string? link = null,
        string? purpose = null,
        string? context = null)
    {
        UserIds = userIds;
        Title = title;
        Message = message;
        Channel = channel;
        Link = link;
        Purpose = purpose;
        Context = context;
    }
} 