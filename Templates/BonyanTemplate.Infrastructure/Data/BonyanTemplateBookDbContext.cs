using Bonyan.AspNetCore.Persistence.EntityFrameworkCore;
using Bonyan.TenantManagement.Domain;
using Bonyan.TenantManagement.Domain.Bonyan.TenantManagement.Domain;
using Bonyan.TenantManagement.EntityFrameworkCore.Bonyan.TenantManagement.EntityFrameworkCore;
using BonyanTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builder;

namespace BonyanTemplate.Infrastructure.Data;

public class BonyanTemplateBookDbContext : BonyanDbContext<BonyanTemplateBookDbContext> , IBonyanTenantDbContext
{

  public BonyanTemplateBookDbContext(DbContextOptions<BonyanTemplateBookDbContext> options,IServiceProvider serviceProvider):base(options,serviceProvider)
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
