using Microsoft.EntityFrameworkCore;

namespace Bonyan.AspNetCore.Persistence.EntityFrameworkCore
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
