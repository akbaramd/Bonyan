using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public interface IBonIdentityManagementDbContext<TUser>: IBonUserManagementDbContext<TUser> where TUser: BonyanUser
{
}