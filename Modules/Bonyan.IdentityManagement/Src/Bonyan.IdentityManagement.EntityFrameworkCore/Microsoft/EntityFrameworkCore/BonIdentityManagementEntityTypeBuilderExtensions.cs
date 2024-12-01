using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore;

public static class BonIdentityManagementEntityTypeBuilderExtensions
{
    public static ModelBuilder ConfigureIdentityManagementModelBuilder<TUser>(this ModelBuilder modelBuilder)
        where TUser : class, IBonIdentityUser
    {
        // Configure User Management base conventions
        modelBuilder.ConfigureUserManagementModelBuilder<TUser>();

        modelBuilder.Entity<TUser>(builder =>
        {
            builder.ConfigureByConvention();
            builder.ConfigurePassword();
        });
        // Configure BonIdentityRole
        modelBuilder.Entity<BonIdentityUserToken>(builder =>
        {
            builder.ConfigureByConvention();
            builder.ToTable("UserTokens"); // Name the table
            builder.HasOne<TUser>()
                .WithMany(x => x.Tokens)
                .HasForeignKey(x => x.UserId);
        });
        // Configure BonIdentityRole
        modelBuilder.Entity<BonIdentityRole>(builder =>
        {
            builder.ConfigureByConvention();
            builder.ToTable("Roles"); // Name the table
            builder.HasMany(role => role.Permissions)
                .WithMany()
                .UsingEntity("RolePermissions"); // Join table for Role-Permission
        });

        // Configure BonIdentityPermission
        modelBuilder.Entity<BonIdentityPermission>(builder =>
        {
            builder.ConfigureByConvention();
            builder.ToTable("Permissions"); // Name the table
            builder.HasKey(permission => permission.Key);
        });

        // Configure BonIdentityUserRoles (Join Table)
        modelBuilder.Entity<BonIdentityUserRoles>(builder =>
        {
            builder.ConfigureByConvention();

            builder.ToTable("UserRoles"); // Name the join table
            builder.HasOne<TUser>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<BonIdentityRole>()
                .WithMany()
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasKey(x => new { x.RoleId, x.UserId });
        });

        return modelBuilder;
    }

    /// <summary>
    /// Configures the password value object.
    /// </summary>
    private static void ConfigurePassword<TUser>(this EntityTypeBuilder<TUser> entity)
        where TUser : class, IBonIdentityUser
    {
        entity.OwnsOne(user => user.Password, password =>
        {
            password.Property(p => p.HashedPassword).HasColumnName("PasswordHash");
            password.Property(p => p.Salt).HasColumnName("PasswordSalt");
        });
    }

    public static ModelBuilder ConfigureIdentityManagementModelBuilder(this ModelBuilder modelBuilder)
    {
        // Configure BonIdentityManagement using BonIdentityUser as the default user type
        modelBuilder.ConfigureIdentityManagementModelBuilder<BonIdentityUser>();
        return modelBuilder;
    }
}