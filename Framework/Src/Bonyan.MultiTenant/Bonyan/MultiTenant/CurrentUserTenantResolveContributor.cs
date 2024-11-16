using Bonyan.User;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.MultiTenant;

public class CurrentUserTenantResolveContributor : TenantResolveContributorBase
{
  public const string ContributorName = "CurrentUser";

  public override string Name => ContributorName;

  public override Task ResolveAsync(ITenantResolveContext context)
  {
    var currentUser = context.ServiceProvider.GetRequiredService<IBonCurrentUser>();
    if (currentUser.IsAuthenticated)
    {
      context.Handled = true;
      context.TenantIdOrName = currentUser.TenantId?.ToString();
    }

    return Task.CompletedTask;
  }
}
