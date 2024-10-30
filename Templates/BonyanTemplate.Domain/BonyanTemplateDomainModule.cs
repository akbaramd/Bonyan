using Bonyan.Layer.Domain;
using Bonyan.Modularity;
using Bonyan.Modularity.Attributes;
using Bonyan.TenantManagement.Domain;
using Module = Bonyan.Modularity.Abstractions.Module;


namespace BonyanTemplate.Domain
{
  [DependOn(typeof(BonyanLayerDomainModule),
    typeof(BonyanTenantManagementDomainModule))]
  public class BonyanTemplateDomainModule : Module
  {
    public override Task OnConfigureAsync(ServiceConfigurationContext context)
    {
      
      return base.OnConfigureAsync(context);
    }
  }
}
