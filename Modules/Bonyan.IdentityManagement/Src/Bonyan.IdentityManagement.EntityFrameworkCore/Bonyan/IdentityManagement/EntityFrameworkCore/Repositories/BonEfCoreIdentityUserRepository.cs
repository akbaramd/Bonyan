using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Layer.Domain;
using Bonyan.UserManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Bonyan.UserManagement.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreIdentityUserRepository<TUser> : BonEfCoreUserRepository<TUser>,IBonIdentityUserRepository<TUser>,IBonIdentityUserReadOnlyRepository<TUser> where TUser : class, IBonIdentityUser
{
  
}

