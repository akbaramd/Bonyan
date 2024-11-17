using Bonyan.Layer.Domain;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.Entities;
using Bonyan.UserManagement.Domain.Users.Repositories;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.UserManagement.EntityFrameworkCore
{
    public class BonEfCoreUserRepository<TUser> : EfCoreBonRepository<TUser, BonUserId, IBonUserManagementDbContext<TUser>>, IBonUserRepository<TUser> where TUser : class, IBonUser
    {
  
        public async Task<TUser?> GetUserByUsernameAsync(string userName)
        {
            return await FindOneAsync(x => x.UserName == userName);
        }

        public async Task<TUser?> GetUserByEmailAsync(BonUserEmail bonUserEmail)
        {
            return await FindOneAsync(x => x.Email == bonUserEmail);
        }
        public async Task<TUser?> GetUserByEmailAsync(string email)
        {
            return await GetUserByEmailAsync(new BonUserEmail(email));
        }

        public async Task<TUser?> GetUserByPhoneNumberAsync(BonUserPhoneNumber bonUserPhoneNumber)
        {
            return await FindOneAsync(x => x.PhoneNumber == bonUserPhoneNumber);
        }
        public async Task<TUser?> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return await GetUserByPhoneNumberAsync(new BonUserPhoneNumber(phoneNumber));
        }
       
    }
    
    public class BonEfCoreUserReadOnlyRepository<TUser> : EfCoreReadonlyRepository<TUser, BonUserId, IBonUserManagementDbContext<TUser>>, IBonUserReadOnlyRepository<TUser> where TUser : class, IBonUser
    {
        public async Task<TUser?> GetUserByUsernameAsync(string userName)
        {
            return await FindOneAsync(x => x.UserName == userName);
        }

        public async Task<TUser?> GetUserByEmailAsync(BonUserEmail bonUserEmail)
        {
            return await FindOneAsync(x => x.Email == bonUserEmail);
        }
        public async Task<TUser?> GetUserByEmailAsync(string email)
        {
            return await GetUserByEmailAsync(new BonUserEmail(email));
        }

        public async Task<TUser?> GetUserByPhoneNumberAsync(BonUserPhoneNumber bonUserPhoneNumber)
        {
            return await FindOneAsync(x => x.PhoneNumber == bonUserPhoneNumber);
        }
        public async Task<TUser?> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return await GetUserByPhoneNumberAsync(new BonUserPhoneNumber(phoneNumber));
        }
       
    }

   
}
