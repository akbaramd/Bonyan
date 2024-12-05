﻿using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.Repositories;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Layer.Domain;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreIdentityRolePermissionsRepository<TUser> :
    EfCoreBonRepository<BonIdentityRolePermissions,IBonIdentityManagementDbContext<TUser>>,IBonIdentityRolePermissionRepository
    where TUser : class, IBonIdentityUser
{
  
}