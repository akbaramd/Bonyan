using Bonyan.DomainDrivenDesign.Domain;
using Bonyan.DomainDrivenDesign.Domain.Events;
using Bonyan.Modularity;
using Bonyan.Modularity.Attributes;
using Bonyan.TenantManagement.Domain;
using Microsoft.Extensions.DependencyInjection;
using Module = Bonyan.Modularity.Abstractions.Module;


namespace BonyanTemplate.Domain
{
  [DependOn(typeof(BonyanDomainDrivenDesignDomainModule),
    typeof(BonyanTenantManagementDomainModule))]
  public class BonyanTemplateDomainModule : Module
  {
    public override Task OnConfigureAsync(ModularityContext context)
    {
      
      return base.OnConfigureAsync(context);
    }
  }
}
