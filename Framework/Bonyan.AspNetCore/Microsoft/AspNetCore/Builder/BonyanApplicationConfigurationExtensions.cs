namespace Microsoft.AspNetCore.Builder;

public static class BonyanApplicationConfigurationExtensions
{
  public static TType? GetConfiguration<TType>(this IBonyanApplicationBuilder applicationBuilder, string key)
  {
    return applicationBuilder.Configuration.GetSection(key).Get<TType>();
  }


  public static string? GetConfiguration(this IBonyanApplicationBuilder applicationBuilder, string key)
  {
    return applicationBuilder.Configuration.GetSection(key).Get<string>();
  }
}
