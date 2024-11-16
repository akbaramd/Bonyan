namespace Bonyan.MultiTenant;


public interface ITenantResolveResultAccessor
{
  TenantResolveResult? Result { get; set; }
}
