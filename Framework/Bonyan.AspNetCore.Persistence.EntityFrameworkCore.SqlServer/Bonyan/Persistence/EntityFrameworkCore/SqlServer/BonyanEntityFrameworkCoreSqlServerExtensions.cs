using Bonyan.AspNetCore.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.Persistence.EntityFrameworkCore.SqlServer;

public static class BonyanEntityFrameworkCoreSqlServerExtensions
{
  public static EntityFrameworkCoreConfiguration UseSqlServer(this EntityFrameworkCoreConfiguration configuration,string connectionStrings) 
  {
    return configuration.Configure(c =>
    {
      c.UseSqlServer(connectionStrings);
    });
  }
}
