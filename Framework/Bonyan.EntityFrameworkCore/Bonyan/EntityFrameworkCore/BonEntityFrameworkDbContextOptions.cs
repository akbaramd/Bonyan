using Microsoft.EntityFrameworkCore;

namespace Bonyan.EntityFrameworkCore
{
  public class BonEntityFrameworkDbContextOptions
  {
    public Action<DbContextOptionsBuilder> DbContextOptionsAction { get; set; } = _ => { };


    public BonEntityFrameworkDbContextOptions Configure(Action<DbContextOptionsBuilder> action) 
    {
      DbContextOptionsAction = action;
      return this;
    }

  }
}
