using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public interface IBonIdentityManagementDbContext<TUser,TRole> : IBonUserManagementDbContext<TUser>
    where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
{
    public DbSet<TRole> Roles { get; set; }
    public DbSet<BonIdentityUserToken<TUser,TRole>> UserTokens { get; set; }
    public DbSet<BonIdentityUserRoles<TUser,TRole>> UserRoles { get; set; }
    public DbSet<BonIdentityUserClaims<TUser,TRole>> UserClaims { get; set; }
    public DbSet<BonIdentityRoleClaims<TRole>> RoleClaims { get; set; }
}


