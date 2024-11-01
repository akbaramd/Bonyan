
using Bonyan.IdentityManagement.Domain;
using Bonyan.UserManagement.Domain;

namespace Microsoft.EntityFrameworkCore;

public static class IdentityManagementEntityTypeBuilderExtensions
{
    public static ModelBuilder ConfigureIdentityManagementByConvention<TUser,TRole>(this ModelBuilder modelBuilder) where TUser: BonyanUser where TRole : BonRole
    {
        modelBuilder.ConfigureUserManagementByConvention<TUser>();
        modelBuilder.Entity<TRole>().ConfigureByConvention();
        modelBuilder.Entity<BonPermission>().ConfigureByConvention();
        modelBuilder.Entity<BonPermission>().HasKey(x=>x.Key);
        modelBuilder.Entity<TRole>().HasMany(x=>x.Permissions).WithMany().UsingEntity("RolePermissions");
        
        modelBuilder.Entity<BonUserRole<TUser,TRole>>().ToTable("UserRole");
        modelBuilder.Entity<BonUserRole<TUser, TRole>>().HasKey(x => x.UserId);
        modelBuilder.Entity<BonUserRole<TUser, TRole>>().HasKey(x => x.RoleId);
        modelBuilder.Entity<BonUserRole<TUser,TRole>>().HasOne(x=>x.User).WithMany().HasForeignKey(x=>x.UserId);
        modelBuilder.Entity<BonUserRole<TUser,TRole>>().HasOne(x=>x.Role).WithMany().HasForeignKey(x=>x.RoleId);
        return modelBuilder;
    }
}