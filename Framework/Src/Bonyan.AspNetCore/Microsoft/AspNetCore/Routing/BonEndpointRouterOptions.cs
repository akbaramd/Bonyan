namespace Microsoft.AspNetCore.Routing;

/// <summary>
/// Options for configuring endpoint routing in Bonyan applications.
/// </summary>
public class BonEndpointRouterOptions
{
    /// <summary>
    /// Gets the list of endpoint configuration actions.
    /// These actions are invoked during endpoint configuration phase.
    /// </summary>
    public List<Action<IEndpointRouteBuilder>> ConfigureActions { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="BonEndpointRouterOptions"/>.
    /// </summary>
    public BonEndpointRouterOptions()
    {
        ConfigureActions = new List<Action<IEndpointRouteBuilder>>();
    }
}