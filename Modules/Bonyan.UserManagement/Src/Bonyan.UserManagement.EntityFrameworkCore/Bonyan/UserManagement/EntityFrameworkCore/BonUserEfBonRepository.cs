using Bonyan.Layer.Domain;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.Entities;
using Bonyan.UserManagement.Domain.Users.Repositories;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.UserManagement.EntityFrameworkCore
{
    public class BonEfCoreUserRepository<TUser> : EfCoreBonRepository<TUser, BonUserId, BonUserManagementDbContext<TUser>>, IBonUserRepository<TUser> where TUser : class, IBonUser
    {
        public BonEfCoreUserRepository(BonUserManagementDbContext<TUser> userManagementDbContext) : base(userManagementDbContext)
        {
        }
       
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
    
    public class BonEfCoreUserReadOnlyRepository<TUser> : EfCoreReadonlyRepository<TUser, BonUserId, BonUserManagementDbContext<TUser>>, IBonUserReadOnlyRepository<TUser> where TUser : class, IBonUser
    {
        public BonEfCoreUserReadOnlyRepository(BonUserManagementDbContext<TUser> userManagementDbContext) : base(userManagementDbContext)
        {
        }
       
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

    public class BonEfCoreUserRepository : EfCoreBonRepository<BonUser, BonUserId, BonUserManagementDbContext>,
        IBonUserRepository
    {
        public BonEfCoreUserRepository(BonUserManagementDbContext context) : base(context)
        {
        }
       
        public async Task<BonUser?> GetUserByUsernameAsync(string userName)
        {
            return await FindOneAsync(x => x.UserName == userName);
        }

        public async Task<BonUser?> GetUserByEmailAsync(BonUserEmail bonUserEmail)
        {
            return await FindOneAsync(x => x.Email == bonUserEmail);
        }
        public async Task<BonUser?> GetUserByEmailAsync(string email)
        {
            return await GetUserByEmailAsync(new BonUserEmail(email));
        }

        public async Task<BonUser?> GetUserByPhoneNumberAsync(BonUserPhoneNumber bonUserPhoneNumber)
        {
            return await FindOneAsync(x => x.PhoneNumber == bonUserPhoneNumber);
        }
        public async Task<BonUser?> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return await GetUserByPhoneNumberAsync(new BonUserPhoneNumber(phoneNumber));
        }
    }
    
    
    public class BonEfCoreUserReadOnlyRepository : EfCoreReadonlyRepository<BonUser, BonUserId, BonUserManagementDbContext>, IBonUserReadOnlyRepository
    {
        public BonEfCoreUserReadOnlyRepository(BonUserManagementDbContext userManagementDbContext) : base(userManagementDbContext)
        {
        }
       
        public async Task<BonUser?> GetUserByUsernameAsync(string userName)
        {
            return await FindOneAsync(x => x.UserName == userName);
        }

        public async Task<BonUser?> GetUserByEmailAsync(BonUserEmail bonUserEmail)
        {
            return await FindOneAsync(x => x.Email == bonUserEmail);
        }
        public async Task<BonUser?> GetUserByEmailAsync(string email)
        {
            return await GetUserByEmailAsync(new BonUserEmail(email));
        }

        public async Task<BonUser?> GetUserByPhoneNumberAsync(BonUserPhoneNumber bonUserPhoneNumber)
        {
            return await FindOneAsync(x => x.PhoneNumber == bonUserPhoneNumber);
        }
        public async Task<BonUser?> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return await GetUserByPhoneNumberAsync(new BonUserPhoneNumber(phoneNumber));
        }
       
    }
}
