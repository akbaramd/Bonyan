using Bonyan.UnitOfWork;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Bonyan.UserManagement;

public class BonUserStore<TUser> : IQueryableUserStore<TUser>, IUserPasswordStore<TUser>
    where TUser : BonyanUser
{
    private readonly IBonyanUserRepository<TUser> _userRepository;

    public BonUserStore(IBonyanUserRepository<TUser> userRepository)
    {
        _userRepository = userRepository;
    }

    public void Dispose()
    {
        
    }

    public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id.ToString());
    }

    public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }

    public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.FromResult(0);
    }

    public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName.ToLower());
    }

    public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
    {
        user.UserName = normalizedName.ToLower();
        return Task.FromResult(0);
    }

    [UnitOfWork]
    public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
    {
        await _userRepository.AddAsync(user);
        return IdentityResult.Success;
    }

    [UnitOfWork]
    public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
    {
        await _userRepository.UpdateAsync(user);
        return IdentityResult.Success;
    }

    [UnitOfWork]
    public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
    {
        await _userRepository.DeleteAsync(user);
        return IdentityResult.Success;
    }

    [UnitOfWork]
    public async Task<TUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await _userRepository.FindOneAsync(x => x.Id.Value.ToString() == userId);
    }

    [UnitOfWork]
    public async Task<TUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return await _userRepository.FindOneAsync(x => x.UserName.ToLower() == normalizedUserName.ToLower());
    }

    public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
    {
        user.SetPassword(passwordHash);
        return Task.FromResult(0);
    }

    public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Password.HashedPassword);
    }

    public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Password != null);
    }

    
    public IQueryable<TUser> Users => _userRepository.Queryable;
}