
using Bonyan.IdentityManagement.Domain;
using Bonyan.UserManagement.Domain;

namespace Microsoft.EntityFrameworkCore;

public static class BonIdentityManagementEntityTypeBuilderExtensions
{
    public static ModelBuilder ConfigureBonIdentityManagementByConvention<TUser,TRole>(this ModelBuilder modelBuilder) where TUser: BonUser where TRole : BonRole
    {
        modelBuilder.ConfigureBonUserManagementByConvention<TUser>();
        modelBuilder.Entity<TRole>().ConfigureByConvention();
        modelBuilder.Entity<BonPermission>().ConfigureByConvention();
        modelBuilder.Entity<BonPermission>().HasKey(x=>x.Key);
        modelBuilder.Entity<TRole>().HasMany(x=>x.Permissions).WithMany().UsingEntity("RolePermissions");
        
        modelBuilder.Entity<BonUserRole<TUser,TRole>>().ToTable("UserRole");
        modelBuilder.Entity<BonUserRole<TUser, TRole>>().HasKey(x => x.BonUserId);
        modelBuilder.Entity<BonUserRole<TUser, TRole>>().HasKey(x => x.BonRoleId);
        modelBuilder.Entity<BonUserRole<TUser,TRole>>().HasOne(x=>x.User).WithMany().HasForeignKey(x=>x.BonUserId);
        modelBuilder.Entity<BonUserRole<TUser,TRole>>().HasOne(x=>x.Role).WithMany().HasForeignKey(x=>x.BonRoleId);
        return modelBuilder;
    }
}