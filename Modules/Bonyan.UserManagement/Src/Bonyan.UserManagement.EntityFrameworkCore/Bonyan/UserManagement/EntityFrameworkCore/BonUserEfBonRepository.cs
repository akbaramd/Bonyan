using Bonyan.Layer.Domain;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Repositories;
using Bonyan.UserManagement.Domain.ValueObjects;

namespace Bonyan.UserManagement.EntityFrameworkCore
{
    public class BonEfCoreUserRepository<TUser> : EfCoreBonRepository<TUser, BonUserId, BonUserManagementDbContext<TUser>>, IBonUserRepository<TUser> where TUser : BonUser
    {
        public BonEfCoreUserRepository(BonUserManagementDbContext<TUser> userManagementDbContext) : base(userManagementDbContext)
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
    
    public class BonEfCoreUserReadOnlyRepository<TUser> : EfCoreReadonlyRepository<TUser, BonUserId, BonUserManagementDbContext<TUser>>, IBonUserReadOnlyRepository<TUser> where TUser : BonUser
    {
        public BonEfCoreUserReadOnlyRepository(BonUserManagementDbContext<TUser> userManagementDbContext) : base(userManagementDbContext)
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

        public async Task<BonUser?> GetUserByEmailAsync(Email email)
        {
            return await FindOneAsync(x => x.Email == email);
        }
        public async Task<BonUser?> GetUserByEmailAsync(string email)
        {
            return await GetUserByEmailAsync(new Email(email));
        }

        public async Task<BonUser?> GetUserByPhoneNumberAsync(PhoneNumber phoneNumber)
        {
            return await FindOneAsync(x => x.PhoneNumber == phoneNumber);
        }
        public async Task<BonUser?> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return await GetUserByPhoneNumberAsync(new PhoneNumber(phoneNumber));
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

        public async Task<BonUser?> GetUserByEmailAsync(Email email)
        {
            return await FindOneAsync(x => x.Email == email);
        }
        public async Task<BonUser?> GetUserByEmailAsync(string email)
        {
            return await GetUserByEmailAsync(new Email(email));
        }

        public async Task<BonUser?> GetUserByPhoneNumberAsync(PhoneNumber phoneNumber)
        {
            return await FindOneAsync(x => x.PhoneNumber == phoneNumber);
        }
        public async Task<BonUser?> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return await GetUserByPhoneNumberAsync(new PhoneNumber(phoneNumber));
        }
       
    }
}
