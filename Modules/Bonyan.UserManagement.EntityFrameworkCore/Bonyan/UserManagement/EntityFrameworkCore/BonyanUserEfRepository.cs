using Bonyan.Layer.Domain;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Repositories;
using Bonyan.UserManagement.Domain.ValueObjects;

namespace Bonyan.UserManagement.EntityFrameworkCore
{
    public class BonyanUserEfRepository<TUser> : EfCoreRepository<TUser, UserId, BonUserManagementDbContext<TUser>>, IBonyanUserRepository<TUser> where TUser : BonyanUser
    {
        public BonyanUserEfRepository(BonUserManagementDbContext<TUser> userManagementDbContext) : base(userManagementDbContext)
        {
        }
       
        public async Task<TUser?> GetUserByUsernameAsync(string userName)
        {
            return await FindOneAsync(x => x.UserName == userName);
        }

        public async Task<TUser?> GetUserByEmailAsync(Email email)
        {
            return await FindOneAsync(x => x.Email == email);
        }
        public async Task<TUser?> GetUserByEmailAsync(string email)
        {
            return await GetUserByEmailAsync(new Email(email));
        }

        public async Task<TUser?> GetUserByPhoneNumberAsync(PhoneNumber phoneNumber)
        {
            return await FindOneAsync(x => x.PhoneNumber == phoneNumber);
        }
        public async Task<TUser?> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return await GetUserByPhoneNumberAsync(new PhoneNumber(phoneNumber));
        }
       
    }
}
