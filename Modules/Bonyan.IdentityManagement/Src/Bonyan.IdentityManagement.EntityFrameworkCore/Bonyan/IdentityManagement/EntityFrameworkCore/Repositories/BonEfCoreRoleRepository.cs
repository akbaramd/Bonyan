using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Abstractions.Roles;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonIdentityEfCoreRoleRepository : EfCoreBonRepository<BonIdentityRole,BonRoleId,BonIdentityManagementDbContext>,IBonIdentityRoleRepository,IBonIdentityRoleReadOnlyRepository
{
    public BonIdentityEfCoreRoleRepository(BonIdentityManagementDbContext userManagementDbContext) : base(userManagementDbContext)
    {
    }
}