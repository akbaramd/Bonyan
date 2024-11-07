namespace Bonyan.DependencyInjection;

public class BonServiceRegistrationActionList : List<Action<IOnServiceRegistredContext>>
{
  public bool IsClassInterceptorsDisabled { get; set; }
}
