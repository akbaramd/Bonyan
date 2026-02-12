using Bonyan.IdentityManagement.Domain;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.TenantManagement.Domain;

namespace BonyanTemplate.Domain;

/// <summary>
/// Domain layer module for the Bonyan Template. Registers domain services and dependencies.
/// </summary>
public class BonyanTemplateDomainModule : BonModule
{
    /// <inheritdoc />
    public BonyanTemplateDomainModule()
    {
        DependOn<BonTenantManagementDomainModule>();
        DependOn<BonIdentityManagementDomainModule>();
    }

    /// <inheritdoc />
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        return base.OnConfigureAsync(context, cancellationToken);
    }
}
