using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.UserMeta;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

/// <summary>
/// DbContext for identity management (non-generic). Uses <see cref="BonIdentityUser"/> and <see cref="BonIdentityRole"/>.
/// </summary>
public interface IBonIdentityManagementDbContext : IBonUserManagementDbContext<BonIdentityUser>
{
    DbSet<BonIdentityRole> Roles { get; set; }
    DbSet<BonIdentityUserToken> UserTokens { get; set; }
    DbSet<BonIdentityUserRoles> UserRoles { get; set; }
    DbSet<BonIdentityUserClaims> UserClaims { get; set; }
    DbSet<BonIdentityRoleClaims> RoleClaims { get; set; }
    DbSet<BonIdentityRoleMeta> RoleMetas { get; set; }
    DbSet<BonUserMeta> UserMetas { get; set; }
}
