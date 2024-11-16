using Bonyan.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.UserManagement.EntityFrameworkCore;

public interface IBonUserManagementDbContext<TUser> where TUser: BonUser
{
  public DbSet<TUser> Users { get; set; }
}
public interface IBonUserManagementDbContext : IBonUserManagementDbContext<BonUser>
{
}
