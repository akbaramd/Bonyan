using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.EntityFrameworkCore;
using Bonyan.Layer.Domain;
using Bonyan.TenantManagement.Domain;
using Bonyan.TenantManagement.EntityFrameworkCore;
using BonyanTemplate.Domain.Authors;
using BonyanTemplate.Domain.Books;
using BonyanTemplate.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace BonyanTemplate.Infrastructure.Data;

public class BonyanTemplateDbContext : BonDbContext<BonyanTemplateDbContext>,
    IBonTenantDbContext,IBonIdentityManagementDbContext
{
    public BonyanTemplateDbContext(DbContextOptions<BonyanTemplateDbContext> options) :
        base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Book>().ConfigureByConvention();
        modelBuilder.Entity<Book>().HasOne(x => x.Author).WithMany().HasForeignKey(x => x.AuthorId);
        modelBuilder.Entity<Authors>().ConfigureByConvention();
        modelBuilder.ConfigureTenantManagementByConvention();
        modelBuilder.ConfigureIdentityManagement<User>();
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Authors> Authors { get; set; }
    public DbSet<BonTenant> Tenants { get; set; }
    public DbSet<BonIdentityUser> Users { get; set; }
    public DbSet<BonIdentityUserToken> UserTokens { get; set; }
    public DbSet<BonIdentityRole> Roles { get; set; }
    public DbSet<BonIdentityUserRoles> UserRoles { get; set; }
    public DbSet<BonIdentityUserClaims> UserClaims { get; set; }
    public DbSet<BonIdentityRoleClaims> RoleClaims { get; set; }
}