namespace Bonyan.DependencyInjection;

/// <summary>
/// List of configuration actions for options, run in order via <see cref="Configure"/>.
/// </summary>
public class BonPreConfigureActionList<TOptions> : List<Action<TOptions>>
{
    public void Configure(TOptions options)
    {
        foreach (var action in this)
        {
            action(options);
        }
    }

    public TOptions Configure()
    {
        var options = Activator.CreateInstance<TOptions>();
        Configure(options);
        return options;
    }
}