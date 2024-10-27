using Bonyan.AspNetCore.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.Persistence.EntityFrameworkCore.SqlServer;

public static class BonyanEntityFrameworkCoreSqlServerExtensions
{
  public static EntityFrameworkDbContextOptions UseSqlServer(this EntityFrameworkDbContextOptions options,string connectionStrings) 
  {
    return options.Configure(c =>
    {
      c.UseSqlServer(connectionStrings);
    });
  }
  
 
}
