using Bonyan.IdentityManagement.Domain.Users.UserMeta;
using Bonyan.Layer.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreUserMetaRepository : EfCoreBonRepository<BonUserMeta, IBonIdentityManagementDbContext>, IBonUserMetaRepository
{
}
