using Bonyan.EntityFrameworkCore;
using Bonyan.UserManagement.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.UserManagement.EntityFrameworkCore;

public interface IBonUserManagementDbContext<TUser> : IEfDbContext where TUser: class, IBonUser
{
  public DbSet<TUser> Users { get; set; }
}
