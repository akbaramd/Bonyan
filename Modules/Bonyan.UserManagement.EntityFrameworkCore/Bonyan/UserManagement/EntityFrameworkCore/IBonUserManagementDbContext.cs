using Bonyan.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.UserManagement.EntityFrameworkCore;

public interface IBonUserManagementDbContext<TUser> where TUser: BonyanUser
{
  public DbSet<TUser> Users { get; set; }
}
