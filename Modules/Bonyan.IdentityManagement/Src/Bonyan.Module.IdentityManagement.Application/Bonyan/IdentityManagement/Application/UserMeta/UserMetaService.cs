using Bonyan.IdentityManagement.Domain.Users.UserMeta;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Bonyan.IdentityManagement.Application.UserMeta;

/// <summary>
/// Application service for user meta (WordPress-style). Other modules can add/update/remove meta per user.
/// </summary>
public class UserMetaService : IUserMetaService
{
    private readonly IBonUserMetaRepository _metaRepository;
    private readonly ILogger<UserMetaService> _logger;

    public UserMetaService(IBonUserMetaRepository metaRepository, ILogger<UserMetaService> logger)
    {
        _metaRepository = metaRepository;
        _logger = logger;
    }

    public async Task<string?> GetAsync(BonUserId userId, string metaKey, CancellationToken cancellationToken = default)
    {
        if (userId == null) throw new ArgumentNullException(nameof(userId));
        if (string.IsNullOrWhiteSpace(metaKey)) throw new ArgumentException("Meta key cannot be null or empty.", nameof(metaKey));

        var meta = await _metaRepository.FindOneAsync(m => m.UserId == userId && m.MetaKey == metaKey);
        return meta?.MetaValue;
    }

    public async Task SetAsync(BonUserId userId, string metaKey, string metaValue, CancellationToken cancellationToken = default)
    {
        if (userId == null) throw new ArgumentNullException(nameof(userId));
        if (string.IsNullOrWhiteSpace(metaKey)) throw new ArgumentException("Meta key cannot be null or empty.", nameof(metaKey));
        metaValue ??= string.Empty;

        var existing = await _metaRepository.FindOneAsync(m => m.UserId == userId && m.MetaKey == metaKey);
        if (existing != null)
        {
            existing.UpdateValue(metaValue);
            await _metaRepository.UpdateAsync(existing, false);
        }
        else
        {
            var meta = new BonUserMeta(userId, metaKey, metaValue);
            await _metaRepository.AddAsync(meta, false);
        }
    }

    public async Task DeleteAsync(BonUserId userId, string metaKey, CancellationToken cancellationToken = default)
    {
        if (userId == null) throw new ArgumentNullException(nameof(userId));
        if (string.IsNullOrWhiteSpace(metaKey)) throw new ArgumentException("Meta key cannot be null or empty.", nameof(metaKey));

        var meta = await _metaRepository.FindOneAsync(m => m.UserId == userId && m.MetaKey == metaKey);
        if (meta != null)
            await _metaRepository.DeleteAsync(meta, false);
    }

    public async Task<IReadOnlyDictionary<string, string>> GetAllAsync(BonUserId userId, CancellationToken cancellationToken = default)
    {
        if (userId == null) throw new ArgumentNullException(nameof(userId));

        var list = await _metaRepository.FindAsync(m => m.UserId == userId);
        return list.ToDictionary(m => m.MetaKey, m => m.MetaValue);
    }
}
