using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Users.UserMeta;

/// <summary>
/// Repository for user meta (WordPress-style key-value per user).
/// </summary>
public interface IBonUserMetaRepository : IBonRepository<BonUserMeta>
{
}
