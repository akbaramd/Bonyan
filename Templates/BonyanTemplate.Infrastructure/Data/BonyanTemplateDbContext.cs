using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.UserMeta;
using Bonyan.IdentityManagement.EntityFrameworkCore;
using Bonyan.Layer.Domain;
using Bonyan.TenantManagement.Domain;
using Bonyan.TenantManagement.EntityFrameworkCore;
using BonyanTemplate.Domain.Authors;
using BonyanTemplate.Domain.Books;
using Microsoft.EntityFrameworkCore;

namespace BonyanTemplate.Infrastructure.Data;

/// <summary>
/// Entity Framework Core DbContext for the Bonyan Template. Configures tenants, identity, books, and authors.
/// </summary>
public class BonyanTemplateDbContext : BonDbContext<BonyanTemplateDbContext>,
    IBonTenantDbContext,
    IBonIdentityManagementDbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BonyanTemplateDbContext"/> class.
    /// </summary>
    public BonyanTemplateDbContext(DbContextOptions<BonyanTemplateDbContext> options)
        : base(options)
    {
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        base.OnConfiguring(optionsBuilder);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Book>().ConfigureByConvention();
        modelBuilder.Entity<Book>().HasOne(x => x.Author).WithMany().HasForeignKey(x => x.AuthorId);
        modelBuilder.Entity<Author>().ConfigureByConvention();
        modelBuilder.ConfigureTenantManagementByConvention();
        modelBuilder.ConfigureIdentityManagement();
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<BonTenant> Tenants { get; set; }


    public DbSet<BonIdentityUser> Users { get; set; }
    public DbSet<BonIdentityRole> Roles { get; set; }
    public DbSet<BonIdentityUserToken> UserTokens { get; set; }
    public DbSet<BonIdentityUserRoles> UserRoles { get; set; }
    public DbSet<BonIdentityUserClaims> UserClaims { get; set; }
    public DbSet<BonIdentityRoleClaims> RoleClaims { get; set; }
    public DbSet<BonIdentityRoleMeta> RoleMetas { get; set; }
    public DbSet<BonUserMeta> UserMetas { get; set; }
}
