using Bonyan.IdentityManagement.Domain;
using Bonyan.Messaging;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.TenantManagement.Domain;
using BonyanTemplate.Domain.Books.Handlers;
using BonyanTemplate.Domain.Users;


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
            PreConfigure<BonMessagingConfiguration>(c =>
            {
                c.RegisterConsumer<BookBonDomainHandler>();
            });
            
            

            return base.OnConfigureAsync(context);
        }
    }
}