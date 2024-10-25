using Bonyan.AspNetCore.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.Persistence.EntityFrameworkCore.Sqlite;

public static class BonyanEntityFrameworkCoreSqliteExtensions
{
  public static EntityFrameworkCoreConfiguration UseSqlite(this EntityFrameworkCoreConfiguration configuration,string connectionStrings) 
  {
    return configuration.Configure(c =>
    {
      c.UseSqlite(connectionStrings);
    });
  }
}
