using Bonyan.IdentityManagement.Domain;
using Bonyan.Layer.Domain;
using Bonyan.Modularity;
using Bonyan.TenantManagement.Domain;
using BonyanTemplate.Domain.DomainEvents;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Module = Bonyan.Modularity.Abstractions.Module;


namespace BonyanTemplate.Domain
{

  public class BonyanTemplateDomainModule : Module
  {
    public BonyanTemplateDomainModule()
    {
      DependOn<BonyanTenantManagementDomainModule>();
      DependOn<BonyanIdentityManagementDomainModule<User>>();
    }
    public override Task OnConfigureAsync(ServiceConfigurationContext context)
    {
      context.AddDomainHandler(typeof(BookDomainHandler));
    
      return base.OnConfigureAsync(context);
    }
  }
}
