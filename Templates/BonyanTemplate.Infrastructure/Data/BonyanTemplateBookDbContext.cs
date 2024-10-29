using Bonyan.EntityFrameworkCore;
using Bonyan.TenantManagement.Domain;
using Bonyan.TenantManagement.EntityFrameworkCore;
using BonyanTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BonyanTemplate.Infrastructure.Data;

public class BonyanTemplateBookDbContext : BonyanDbContext<BonyanTemplateBookDbContext> , IBonyanTenantDbContext
{

  public BonyanTemplateBookDbContext(DbContextOptions<BonyanTemplateBookDbContext> options):base(options)
  {

  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<Books>().ConfigureByConvention();
    modelBuilder.Entity<Authors>().ConfigureByConvention();
    modelBuilder.ConfigureTenantManagementByConvention();
    
   
  }

  public DbSet<Books> Books { get; set; }
  public DbSet<Authors> Authors { get; set; }
  public DbSet<Tenant> Tenants { get; set; }
}
