namespace Bonyan.DependencyInjection;

/// <summary>
/// List of actions invoked when service types are exposed (e.g. for proxy generation).
/// </summary>
public class BonServiceExposingActionList : List<Action<IOnServiceExposingContext>>
{
}