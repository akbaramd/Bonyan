using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Application.UserMeta;

/// <summary>
/// WordPress-style user meta: get/set/delete key-value data per user so other modules can extend user data.
/// </summary>
public interface IUserMetaService
{
    Task<string?> GetAsync(BonUserId userId, string metaKey, CancellationToken cancellationToken = default);
    Task SetAsync(BonUserId userId, string metaKey, string metaValue, CancellationToken cancellationToken = default);
    Task DeleteAsync(BonUserId userId, string metaKey, CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<string, string>> GetAllAsync(BonUserId userId, CancellationToken cancellationToken = default);
}
