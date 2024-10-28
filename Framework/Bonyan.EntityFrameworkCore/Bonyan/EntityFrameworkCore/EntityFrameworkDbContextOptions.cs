using Microsoft.EntityFrameworkCore;

namespace Bonyan.EntityFrameworkCore
{
  public class EntityFrameworkDbContextOptions
  {
    public Action<DbContextOptionsBuilder> DbContextOptionsAction { get; set; } = _ => { };


    public EntityFrameworkDbContextOptions Configure(Action<DbContextOptionsBuilder> action) 
    {
      DbContextOptionsAction = action;
      return this;
    }

  }
}
