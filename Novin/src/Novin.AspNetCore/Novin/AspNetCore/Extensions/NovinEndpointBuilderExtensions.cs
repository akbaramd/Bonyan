using Novin.AspNetCore.Novin.AspNetCore.Endpoints;

namespace Novin.AspNetCore.Novin.AspNetCore.Extensions;

public static class NovinEndpointBuilderExtensions
{
    // No need to change this method as it only adds endpoints if necessary
    public static NovinConfigurationContext AddEndpoints(this NovinConfigurationContext builder)
    {
        return builder;
    }
    
    // Updated UseEndpoints to use top-level route registration
    public static NovinApplicationContext UseEndpoints(this NovinApplicationContext context, Action<NonEndpointBuilder> endpointBuilder)
    {
        var nonEndpointBuilder = new NonEndpointBuilder(context.BonAppContext.Application);
        endpointBuilder.Invoke(nonEndpointBuilder);
        return context;
    }
}
