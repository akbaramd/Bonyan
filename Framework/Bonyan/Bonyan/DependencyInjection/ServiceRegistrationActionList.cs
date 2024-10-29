namespace Bonyan.DependencyInjection;

public class ServiceRegistrationActionList : List<Action<IOnServiceRegistredContext>>
{
  public bool IsClassInterceptorsDisabled { get; set; }
}
