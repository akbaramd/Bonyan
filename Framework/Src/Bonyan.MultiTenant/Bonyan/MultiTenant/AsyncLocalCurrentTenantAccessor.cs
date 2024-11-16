namespace Bonyan.MultiTenant;

public class AsyncLocalCurrentTenantAccessor : ICurrentTenantAccessor
{
  public static AsyncLocalCurrentTenantAccessor Instance { get; } = new();

  public BonBasicTenantInfo? Current {
    get => _currentScope.Value;
    set => _currentScope.Value = value;
  }

  private readonly AsyncLocal<BonBasicTenantInfo?> _currentScope;

  private AsyncLocalCurrentTenantAccessor()
  {
    _currentScope = new AsyncLocal<BonBasicTenantInfo?>();
  }
}
