using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore
{
    public static class BonIdentityManagementEntityTypeBuilderExtensions
    {
        public static ModelBuilder ConfigureIdentityManagementModelBuilder<TUser>(this ModelBuilder modelBuilder)
            where TUser : class, IBonIdentityUser
        {
            // Configure User Management base conventions
            modelBuilder.ConfigureUserManagementModelBuilder<TUser>();

            // Configure BonIdentityUser
            modelBuilder.Entity<TUser>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ConfigurePassword();
            });

            // Configure BonIdentityUserToken
            modelBuilder.Entity<BonIdentityUserToken>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("UserTokens"); // Name the table
                builder.HasOne<TUser>()
                    .WithMany(x => x.Tokens)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade); // Ensure proper delete behavior
            });

            // Configure BonIdentityRole
            modelBuilder.Entity<BonIdentityRole>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("Roles"); // Name the table
            });

            // Configure BonIdentityPermission
            modelBuilder.Entity<BonIdentityPermission>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("Permissions"); // Name the table
            });

            // Configure BonIdentityUserRoles (Join Table)
            modelBuilder.Entity<BonIdentityUserRoles>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("UserRoles"); // Name the join table

                builder.HasOne<TUser>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade); // Cascade delete may be adjusted as needed

                builder.HasOne<BonIdentityRole>()
                    .WithMany()
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasKey(x => new { x.RoleId, x.UserId });
            });

            // Configure BonIdentityRolePermissions (Join Table)
            modelBuilder.Entity<BonIdentityRolePermissions>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("RolePermissions"); // Name the join table

                builder.HasOne<BonIdentityRole>()
                    .WithMany()
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne<BonIdentityPermission>()
                    .WithMany()
                    .HasForeignKey(e => e.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasKey(x => new { x.RoleId, x.PermissionId });
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
}
