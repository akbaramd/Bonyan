using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.OutBox.HostedServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bonyan.Messaging.OutBox
{
    /// <summary>
    /// Module for configuring the outbox pattern in the messaging system.
    /// This module extends the messaging configuration to support reliable messaging.
    /// </summary>
    public class BonMessagingOutboxModule : BonModule
    {
        public BonMessagingOutboxModule()
        {
            DependOn<BonMessagingModule>();
        }

        public override Task OnConfigureAsync(BonConfigurationContext context)
        {
            // Register outbox services
            context.Services.AddSingleton<IOutboxStore, InMemoryOutboxStore>();
            
            // Register BonServiceManager if not already registered
            if (!context.Services.Any(s => s.ServiceType == typeof(BonServiceManager)))
            {
                context.Services.AddSingleton<BonServiceManager>();
            }
            
            // Register a default producer if not already registered
            if (!context.Services.Any(s => s.ServiceType == typeof(IBonMessageProducer)))
            {
                context.Services.AddSingleton<IBonMessageProducer, DefaultMessageProducer>();
            }
            
            context.Services.Replace(ServiceDescriptor.Singleton<IBonMessageBus, BonOutBoxMessageBox>());
            context.Services.AddHostedService<OutboxProcessorHostedService>();
            
            return base.OnConfigureAsync(context);
        }
    }
    
    /// <summary>
    /// Default implementation of IBonMessageProducer for outbox pattern.
    /// This is a no-op implementation that can be replaced with actual producers.
    /// </summary>
    internal class DefaultMessageProducer : IBonMessageProducer
    {
        public Task PublishAsync<TMessage>(
            string serviceName, 
            TMessage message, 
            IDictionary<string, object>? headers = null, 
            string? correlationId = null, 
            string? replyQueue = null, 
            CancellationToken cancellationToken = default) 
            where TMessage : class
        {
            // Default implementation - just return completed task
            // In a real scenario, this would be replaced with RabbitMQ or other producers
            return Task.CompletedTask;
        }
    }
}