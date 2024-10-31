using Bonyan.IdentityManagement.Domain;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public interface IBonIdentityManagementDbContext<TUser,TRole>: IBonUserManagementDbContext<TUser> where TUser: BonyanUser where TRole: BonRole
{
    public DbSet<TRole> Roles { get; set; }
}