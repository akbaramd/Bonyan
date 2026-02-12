using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.UserMeta;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore
{
    public static class BonIdentityManagementEntityTypeBuilderExtensions
    {
        public static ModelBuilder ConfigureIdentityManagement(this ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureUserManagement<BonIdentityUser>(builder =>
            {
                builder.ConfigurePassword();

                builder.HasMany(x => x.Metas)
                    .WithOne()
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasIndex(x => x.UserName)
                    .HasDatabaseName("IX_Users_UserName")
                    .IsUnique();

                builder.HasIndex(x => x.CreatedAt)
                    .HasDatabaseName("IX_Users_CreatedAt");

                builder.Property(x => x.UserName)
                    .HasMaxLength(256)
                    .IsRequired();
            });

            modelBuilder.Entity<BonIdentityUserToken>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("UserTokens");

                builder.Property(x => x.Id).ValueGeneratedNever();

                builder.Property(x => x.Type).HasMaxLength(100).IsRequired();
                builder.Property(x => x.Value).HasMaxLength(256).IsRequired(); // Hashed value only

                builder.HasIndex(x => new { x.UserId, x.Type })
                    .HasDatabaseName("IX_UserTokens_UserId_Type")
                    .IsUnique();

                builder.HasIndex(x => x.Expiration)
                    .HasDatabaseName("IX_UserTokens_Expiration")
                    .HasFilter("Expiration IS NOT NULL");

                builder.HasIndex(x => x.CreatedAt).HasDatabaseName("IX_UserTokens_CreatedAt");
            });

            modelBuilder.Entity<BonIdentityRole>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("Roles");

                builder.Property(x => x.Title).HasMaxLength(256).IsRequired();

                builder.HasIndex(x => x.Title)
                    .HasDatabaseName("IX_Roles_Title")
                    .IsUnique();

                builder.HasIndex(x => x.CanBeDeleted).HasDatabaseName("IX_Roles_CanBeDeleted");

                builder.HasMany(x => x.RoleClaims)
                    .WithOne(x => x.Role)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(x => x.RoleMetas)
                    .WithOne()
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BonIdentityRoleMeta>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("RoleMetas");

                builder.HasKey(x => new { x.RoleId, x.MetaKey });

                builder.Property(x => x.MetaKey).HasMaxLength(256).IsRequired();
                builder.Property(x => x.MetaValue).HasMaxLength(4000);

                builder.HasIndex(x => x.RoleId).HasDatabaseName("IX_RoleMetas_RoleId");
                builder.HasIndex(x => new { x.RoleId, x.MetaKey })
                    .HasDatabaseName("IX_RoleMetas_RoleId_MetaKey")
                    .IsUnique();
            });

            modelBuilder.Entity<BonIdentityUserRoles>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("UserRoles");

                builder.HasKey(r => new { r.UserId, r.RoleId });

                builder.HasIndex(x => x.RoleId).HasDatabaseName("IX_UserRoles_RoleId");
                builder.HasIndex(x => x.UserId).HasDatabaseName("IX_UserRoles_UserId");

                builder.HasOne<BonIdentityUser>()
                    .WithMany(x => x.UserRoles)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne<BonIdentityRole>()
                    .WithMany()
                    .HasForeignKey(r => r.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BonIdentityUserClaims>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("UserClaims");

                builder.Property(x => x.Id).ValueGeneratedNever();

                builder.Property(x => x.ClaimType).HasMaxLength(256).IsRequired();
                builder.Property(x => x.ClaimValue).HasMaxLength(1000).IsRequired();
                builder.Property(x => x.Issuer).HasMaxLength(256);

                builder.HasIndex(x => new { x.UserId, x.ClaimType }).HasDatabaseName("IX_UserClaims_UserId_ClaimType");
                builder.HasIndex(x => x.ClaimType).HasDatabaseName("IX_UserClaims_ClaimType");
                builder.HasIndex(x => x.ClaimValue).HasDatabaseName("IX_UserClaims_ClaimValue");
                builder.HasIndex(x => new { x.UserId, x.ClaimType, x.ClaimValue })
                    .HasDatabaseName("IX_UserClaims_UserId_ClaimType_ClaimValue")
                    .IsUnique();
            });

            modelBuilder.Entity<BonIdentityRoleClaims>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("RoleClaims");

                builder.Property(x => x.Id).ValueGeneratedNever();

                builder.Property(x => x.ClaimType).HasMaxLength(256).IsRequired();
                builder.Property(x => x.ClaimValue).HasMaxLength(1000).IsRequired();
                builder.Property(x => x.Issuer).HasMaxLength(256);

                builder.HasIndex(x => new { x.RoleId, x.ClaimType }).HasDatabaseName("IX_RoleClaims_RoleId_ClaimType");
                builder.HasIndex(x => x.ClaimType).HasDatabaseName("IX_RoleClaims_ClaimType");
                builder.HasIndex(x => x.ClaimValue).HasDatabaseName("IX_RoleClaims_ClaimValue");
                builder.HasIndex(x => new { x.RoleId, x.ClaimType, x.ClaimValue })
                    .HasDatabaseName("IX_RoleClaims_RoleId_ClaimType_ClaimValue")
                    .IsUnique();

                builder.HasOne(x => x.Role)
                    .WithMany(x => x.RoleClaims)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BonUserMeta>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("UserMetas");

                builder.HasKey(x => new { x.UserId, x.MetaKey });

                builder.Property(x => x.MetaKey).HasMaxLength(256).IsRequired();
                builder.Property(x => x.MetaValue).HasMaxLength(4000);

                builder.HasIndex(x => x.UserId).HasDatabaseName("IX_UserMetas_UserId");
                builder.HasIndex(x => new { x.UserId, x.MetaKey })
                    .HasDatabaseName("IX_UserMetas_UserId_MetaKey")
                    .IsUnique();
            });

            return modelBuilder;
        }

        private static void ConfigurePassword(this EntityTypeBuilder<BonIdentityUser> entity)
        {
            entity.OwnsOne(user => user.Password, password =>
            {
                password.Property(p => p.HashedPassword)
                    .HasColumnName("PasswordHash")
                    .HasMaxLength(256)
                    .IsRequired();

                password.Property(p => p.Salt)
                    .HasColumnName("PasswordSalt")
                    .IsRequired();
            });
        }
    }
}
