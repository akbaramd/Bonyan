using BonyanTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BonyanTemplate.Persistence;

public class AppDbContext : DbContext
{

  public DbSet<Books> Books { get; set; }
  public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
  {
    
  }
}
