namespace Microsoft.AspNetCore.Builder;

public interface IBonyanApplicationInitializer
{
  Task InitializeAsync(BonyanApplication application);
}
