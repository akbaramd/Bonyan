using Bonyan.Layer.Domain;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Enumerations;
using Bonyan.UserManagement.Domain.Repositories;
using Bonyan.UserManagement.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace Bonyan.UserManagement.EntityFrameworkCore
{
    public class BonyanUserEfRepository<TUser> : EfCoreRepository<TUser, UserId, BonUserManagementDbContext<TUser>>, IBonyanUserRepository<TUser> where TUser : BonyanUser
    {
        public BonyanUserEfRepository(BonUserManagementDbContext<TUser> userManagementDbContext) : base(userManagementDbContext)
        {
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

        public void Dispose()
        {
            
        }
    }
}
