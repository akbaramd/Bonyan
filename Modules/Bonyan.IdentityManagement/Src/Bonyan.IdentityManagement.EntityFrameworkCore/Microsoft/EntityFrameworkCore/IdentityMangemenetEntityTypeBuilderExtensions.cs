﻿
using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;

namespace Microsoft.EntityFrameworkCore;

public static class BonIdentityManagementEntityTypeBuilderExtensions
{
    public static ModelBuilder ConfigureBonIdentityManagementByConvention<TUser>(this ModelBuilder modelBuilder) where TUser: class, IBonIdentityUser 
    {
        modelBuilder.ConfigureBonUserManagementByConvention<TUser>();
        modelBuilder.Entity<BonIdentityRole>().ConfigureByConvention();
        modelBuilder.Entity<BonIdentityPermission>().ConfigureByConvention();
        modelBuilder.Entity<BonIdentityPermission>().HasKey(x=>x.Key);
        modelBuilder.Entity<BonIdentityRole>().HasMany(x=>x.Permissions).WithMany().UsingEntity("RolePermissions");
               
        modelBuilder.Entity<TUser>()
            .HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity("UserRoles");
        return modelBuilder;
    }
    
    public static ModelBuilder ConfigureBonIdentityManagementByConvention(this ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureBonIdentityManagementByConvention<BonIdentityUser>();
        return modelBuilder;
    }
}