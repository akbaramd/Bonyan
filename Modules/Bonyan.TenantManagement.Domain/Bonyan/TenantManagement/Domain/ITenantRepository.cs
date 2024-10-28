using Bonyan.DomainDrivenDesign.Domain.Abstractions;

namespace Bonyan.TenantManagement.Domain;

public interface ITenantRepository : IRepository<Tenant,TenantId>
{
  
}
