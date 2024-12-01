using Bonyan.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Novin.AspNetCore.Novin.AspNetCore.Messaging
{
    public static class NovinMessagingEndpointBuilderExtensions
    {
        /// <summary>
        /// Adds Mediator services to the configuration context.
        /// </summary>
        public static NovinConfigurationContext AddMessaging(this NovinConfigurationContext builder,
            Action<BonMessagingConfiguration> configureOptions )
        {
            builder.BonContext.AddMessaging(configureOptions);
            return builder;
        }

        public static NovinApplicationContext UseMessagingEndpoints(this NovinApplicationContext context, Action<NovinMessagingEndpointBuilder> endpointBuilder)
        {
            var nonEndpointBuilder = new NovinMessagingEndpointBuilder(context.BonAppContext.Application);
            endpointBuilder.Invoke(nonEndpointBuilder);
            return context;
        }
        
    }
}
