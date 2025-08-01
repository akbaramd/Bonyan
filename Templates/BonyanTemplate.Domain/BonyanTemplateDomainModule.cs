using Bonyan.IdentityManagement.Domain;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.TenantManagement.Domain;
using BonyanTemplate.Domain.Users;


namespace BonyanTemplate.Domain
{
    public class BonyanTemplateDomainModule : BonModule
    {
        public BonyanTemplateDomainModule()
        {
            DependOn<BonTenantManagementDomainModule>();
            DependOn<BonIdentityManagementDomainModule<User,Role>>();
        }

        public override Task OnConfigureAsync(BonConfigurationContext context)
        {
           
            
            

            return base.OnConfigureAsync(context);
        }
    }
}