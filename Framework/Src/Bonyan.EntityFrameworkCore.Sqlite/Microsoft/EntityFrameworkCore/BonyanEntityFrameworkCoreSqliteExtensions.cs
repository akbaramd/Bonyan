using Bonyan.EntityFrameworkCore.Abstractions;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore;

public static class BonyanEntityFrameworkCoreSqliteExtensions
{
  public static IBonDbContextRegistrationOptionBuilder UseSqlite(this IBonDbContextRegistrationOptionBuilder options,string connectionStrings,Action<SqliteDbContextOptionsBuilder>? sqliteOptionsAction = null)  
  {
    return options.Configure(c =>
    {
      c.UseSqlite(connectionStrings, c =>
      {
        c.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        
        sqliteOptionsAction?.Invoke(c);
      });

    });
  }
}
