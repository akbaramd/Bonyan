using Bonyan.Layer.Domain.DomainService;
using JetBrains.Annotations;

namespace Bonyan.TenantManagement.Domain;

public interface IBonTenantManager : IBonDomainService
{
  Task<BonTenant> CreateAsync([NotNull] string name);
  Task<BonTenant> DeleteAsync([NotNull] string name);

  Task ChangeNameAsync([NotNull] BonTenant bonTenant, [NotNull] string key);
}
