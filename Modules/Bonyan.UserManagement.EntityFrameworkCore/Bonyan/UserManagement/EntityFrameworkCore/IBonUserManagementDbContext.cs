using Bonyan.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public interface IBonUserDbContext<TUser> where TUser: BonyanUser
{
  public DbSet<TUser> Users { get; set; }
}
