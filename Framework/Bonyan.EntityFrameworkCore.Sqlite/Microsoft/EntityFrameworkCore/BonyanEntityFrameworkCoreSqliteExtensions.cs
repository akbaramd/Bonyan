using Bonyan.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore;

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
