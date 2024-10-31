using Bonyan.Layer.Domain;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Enumerations;
using Bonyan.UserManagement.Domain.Repositories;
using Bonyan.UserManagement.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace Bonyan.UserManagement.EntityFrameworkCore
{
    public class BonyanUserEfRepository<TUser> : EfCoreRepository<TUser, UserId, BonUserManagementDbContext<TUser>>, IBonyanUserRepository<TUser>,IUserPasswordStore<TUser> where TUser : BonyanUser
    {
        public BonyanUserEfRepository(BonUserManagementDbContext<TUser> userManagementDbContext) : base(userManagementDbContext)
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
            return UpdateUserAsync(user);
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName.ToLower());
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.UserName = normalizedName.ToLower();
            return UpdateUserAsync(user);
        }

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            await AddAsync(user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            await UpdateAsync(user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            await DeleteAsync(user);
            return IdentityResult.Success;
        }

        public async Task<TUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await FindOneAsync(x => x.Id.Value.ToString() == userId);
        }

        public async Task<TUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await FindOneAsync(x => x.UserName.ToLower() == normalizedUserName.ToLower());
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.SetPassword(passwordHash);
            return UpdateUserAsync(user);
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Password.HashedPassword);
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Password != null);
        }

        public async Task<TUser?> GetUserByUsernameAsync(string userName)
        {
            return await GetOneAsync(x => x.UserName == userName);
        }

        public async Task<TUser?> GetUserByEmailAsync(Email email)
        {
            return await GetOneAsync(x => x.Email == email);
        }

        public async Task<TUser?> GetUserByPhoneNumberAsync(PhoneNumber phoneNumber)
        {
            return await GetOneAsync(x => x.PhoneNumber == phoneNumber);
        }

        public async Task<IReadOnlyList<TUser>> GetUsersByStatusAsync(UserStatus status)
        {
            return (await FindAsync(x => x.Status == status)).ToList().AsReadOnly();
        }

        public async Task UpdateUserAsync(TUser user)
        {
            await UpdateAsync(user);
        }

        public async Task DeleteUserAsync(TUser user)
        {
            await DeleteAsync(user);
        }

        public void Dispose()
        {
        }
    }
}
