using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.UserManagement.EntityFrameworkCore;

public interface IBonUserManagementDbContext<TUser> where TUser: class, IBonUser
{
  public DbSet<TUser> Users { get; set; }
}
public interface IBonUserManagementDbContext : IBonUserManagementDbContext<BonUser>
{
}
