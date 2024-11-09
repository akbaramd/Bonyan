using Bonyan.IdentityManagement.Domain;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.TenantManagement.Domain;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Handlers;
using Microsoft.Extensions.DependencyInjection;


namespace BonyanTemplate.Domain
{

  public class BonyanTemplateDomainModule : BonModule
  {
    public BonyanTemplateDomainModule()
    {
      DependOn<BonTenantManagementDomainModule>();
      DependOn<BonIdentityManagementDomainModule<User>>();
    }
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
      context.AddDomainHandler(typeof(BookBonDomainHandler));
    
      return base.OnConfigureAsync(context);
    }
  }
}
