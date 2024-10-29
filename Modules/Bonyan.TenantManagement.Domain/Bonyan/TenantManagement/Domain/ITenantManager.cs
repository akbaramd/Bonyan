using Bonyan.Layer.Domain.Services;
using JetBrains.Annotations;

namespace Bonyan.TenantManagement.Domain;

public interface ITenantManager : IDomainService
{
  Task<Tenant> CreateAsync([NotNull] string name);

  Task ChangeNameAsync([NotNull] Tenant tenant, [NotNull] string key);
}
