using Bonyan.AspNetCore.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.Persistence.EntityFrameworkCore.Sqlite;

public static class BonyanEntityFrameworkCoreSqliteExtensions
{
  public static EntityFrameworkDbContextOptions UseSqlite(this EntityFrameworkDbContextOptions options,string connectionStrings)  
  {
    return options.Configure(c =>
    {
      c.UseSqlite(connectionStrings);
    });
  }
}
