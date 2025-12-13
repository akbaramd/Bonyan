using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Security.Claims;
using Bonyan.User;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Security;

public class BonSecurityModule : BonModule
{
  public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
  {
    context.Services.AddSingleton<ThreadBonCurrentPrincipalAccessor>();
    context.Services.AddTransient<IBonCurrentUser, BonCurrentUser>();
    return base.OnConfigureAsync(context, cancellationToken);
  }

}