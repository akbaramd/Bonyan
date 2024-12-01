using Bonyan.Mediators;
using Microsoft.Extensions.DependencyInjection;

namespace Novin.AspNetCore.Novin.AspNetCore.Mediator
{
    public static class NovinMediatorEndpointBuilderExtensions
    {
        /// <summary>
        /// Adds Mediator services to the configuration context.
        /// </summary>
        public static NovinConfigurationContext AddMediator(this NovinConfigurationContext builder,
            Action<BonMediatorConfiguration>? configureOptions = null)
        {
            builder.BonContext.AddMediator(configureOptions);
            return builder;
        }

        public static NovinApplicationContext UseMediatorEndpoints(this NovinApplicationContext context, Action<NovinMediatorEndpointBuilder> endpointBuilder)
        {
            var nonEndpointBuilder = new NovinMediatorEndpointBuilder(context.BonAppContext.Application);
            endpointBuilder.Invoke(nonEndpointBuilder);
            return context;
        }
        
    }
}
