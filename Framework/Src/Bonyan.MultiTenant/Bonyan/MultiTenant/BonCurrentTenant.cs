using Bonyan.Core;

namespace Bonyan.MultiTenant;

public class BonCurrentTenant : IBonCurrentTenant
{
  public virtual bool IsAvailable => Id.HasValue;

  public virtual Guid? Id => _currentTenantAccessor.Current?.TenantId;

  public string? Name => _currentTenantAccessor.Current?.Name;

  private readonly ICurrentTenantAccessor _currentTenantAccessor;

  public BonCurrentTenant(ICurrentTenantAccessor currentTenantAccessor)
  {
    _currentTenantAccessor = currentTenantAccessor;
  }

  public IDisposable Change(Guid? id, string? name = null)
  {
    return SetCurrent(id, name);
  }

  private IDisposable SetCurrent(Guid? tenantId, string? name = null)
  {
    var parentScope = _currentTenantAccessor.Current;
    _currentTenantAccessor.Current = new BonBasicTenantInfo(tenantId, name);

    return new DisposeAction<ValueTuple<ICurrentTenantAccessor, BonBasicTenantInfo?>>(static (state) =>
    {
      var (currentTenantAccessor, parentScope) = state;
      currentTenantAccessor.Current = parentScope;
    }, (_currentTenantAccessor, parentScope));
  }
}
