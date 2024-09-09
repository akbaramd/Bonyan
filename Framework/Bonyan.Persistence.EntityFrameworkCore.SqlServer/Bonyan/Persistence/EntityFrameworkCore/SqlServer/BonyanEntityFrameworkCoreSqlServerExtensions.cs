using Microsoft.EntityFrameworkCore;

namespace Bonyan.Persistence.EntityFrameworkCore.SqlServer;

public static class BonyanEntityFrameworkCoreSqlServerExtensions
{
  public static EfCoreConfiguration<TDbContext> UseSqlServer<TDbContext>(this EfCoreConfiguration<TDbContext> configuration,string connectionStrings) where TDbContext : DbContext
  {
    return configuration.Configure(c =>
    {
      c.UseSqlServer(connectionStrings);
    });
  }
}
