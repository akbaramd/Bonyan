using Bonyan.AspNetCore.Persistence.EntityFrameworkCore;
using BonyanTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builder;

namespace BonyanTemplate.Infrastructure.Data;

public class BonyanTemplateBookDbContext : BonyanDbContext<BonyanTemplateBookDbContext>
{

  public BonyanTemplateBookDbContext(DbContextOptions<BonyanTemplateBookDbContext> options):base(options)
  {
    
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<Books>().ConfigureByConvention();
    modelBuilder.Entity<Authors>().ConfigureByConvention();
  }

  public DbSet<Books> Books { get; set; }
  public DbSet<Authors> Authors { get; set; }
  
  
}
