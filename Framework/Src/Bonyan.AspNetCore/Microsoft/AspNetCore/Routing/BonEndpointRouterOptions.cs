namespace Microsoft.AspNetCore.Routing;

public class BonEndpointRouterOptions
{
    public List<Action<EndpointRouteBuilderContext>> EndpointConfigureActions { get; }

    public BonEndpointRouterOptions()
    {
        EndpointConfigureActions = new List<Action<EndpointRouteBuilderContext>>();
    }
}