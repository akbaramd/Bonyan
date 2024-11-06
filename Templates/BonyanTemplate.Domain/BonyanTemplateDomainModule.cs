using Bonyan.IdentityManagement.Domain;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.TenantManagement.Domain;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Handlers;
using Microsoft.Extensions.DependencyInjection;


namespace BonyanTemplate.Domain
{

  public class BonyanTemplateDomainModule : Module
  {
    public BonyanTemplateDomainModule()
    {
      DependOn<BonyanTenantManagementDomainModule>();
      DependOn<BonyanIdentityManagementDomainModule<User,Role>>();
    }
    public override Task OnConfigureAsync(ServiceConfigurationContext context)
    {
      context.AddDomainHandler(typeof(BookDomainHandler));
    
      return base.OnConfigureAsync(context);
    }
  }
}
