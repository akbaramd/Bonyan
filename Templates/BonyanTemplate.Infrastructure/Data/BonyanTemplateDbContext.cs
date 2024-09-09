using BonyanTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BonyanTemplate.Infrastructure.Data;

public class BonyanTemplateDbContext : DbContext
{

  public BonyanTemplateDbContext(DbContextOptions<BonyanTemplateDbContext> options):base(options)
  {
    
  }
  
  public DbSet<Books> Books { get; set; }
  
  
}
