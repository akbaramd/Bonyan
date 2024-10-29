using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.User;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Security;

public class BonyanSecurityModule : Module
{
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    context.Services.AddTransient<ICurrentUser, CurrentUser>();
    return base.OnConfigureAsync(context);
  }

}