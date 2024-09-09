using Bonyan.AspNetCore.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.Persistence.EntityFrameworkCore.Sqlite;

public static class BonyanEntityFrameworkCoreSqliteExtensions
{
  public static EfCoreConfiguration<TDbContext> UseSqlite<TDbContext>(this EfCoreConfiguration<TDbContext> configuration,string connectionStrings) where TDbContext : DbContext
  {
    return configuration.Configure(c =>
    {
      c.UseSqlite(connectionStrings);
    });
  }
}
