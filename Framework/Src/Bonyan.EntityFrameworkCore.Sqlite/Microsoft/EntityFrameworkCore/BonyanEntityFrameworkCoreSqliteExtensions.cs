using Bonyan.EntityFrameworkCore;
using Bonyan.EntityFrameworkCore.Abstractions;

namespace Microsoft.EntityFrameworkCore;

public static class BonyanEntityFrameworkCoreSqliteExtensions
{
  public static IBonDbContextRegistrationOptionBuilder UseSqlite(this IBonDbContextRegistrationOptionBuilder options,string connectionStrings)  
  {
    return options.Configure(c =>
    {
      c.UseSqlite(connectionStrings);
    });
  }
}
