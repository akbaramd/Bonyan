using Bonyan.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore;

public static class BonyanEntityFrameworkCoreSqliteExtensions
{
  public static BonEntityFrameworkDbContextOptions UseSqlite(this BonEntityFrameworkDbContextOptions options,string connectionStrings)  
  {
    return options.Configure(c =>
    {
      c.UseSqlite(connectionStrings);
    });
  }
}
