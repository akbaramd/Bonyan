using Bonyan.IdentityManagement.Domain;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public interface IBonIdentityManagementDbContext<TUser, TRole> : IBonUserManagementDbContext<TUser>
    where TUser : BonUser where TRole : BonRole
{
    public DbSet<TRole> Roles { get; set; }
    public DbSet<BonPermission> Permissions { get; set; }
    public DbSet<BonUserRole<TUser, TRole>> UserRoles { get; set; }
}