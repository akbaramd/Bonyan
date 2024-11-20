using Bonyan.Layer.Domain.DomainService;
using Bonyan.Layer.Domain.Services;
using Bonyan.UserManagement.Domain.Users.Repositories;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Microsoft.Extensions.Logging;

namespace Bonyan.UserManagement.Domain.Users.DomainServices;

public class BonUserManager<TUser> : BonDomainService, IBonUserManager<TUser> where TUser : class, IBonUser
{
    public IBonUserRepository<TUser> UserRepository =>
        LazyServiceProvider.LazyGetRequiredService<IBonUserRepository<TUser>>();

    // Create a new user
    public async Task<BonDomainResult> CreateAsync(TUser entity)
    {
        try
        {
            if (await UserRepository.ExistsAsync(x => x.UserName.Equals(entity.UserName)))
            {
                Logger.LogWarning($"User with username {entity.UserName} already exists.");
                return BonDomainResult.Failure($"User with username {entity.UserName} already exists.");
            }

            entity.Id = BonUserId.NewId();
            await UserRepository.AddAsync(entity, true);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error creating user.");
            return BonDomainResult.Failure(e.Message);
        }
    }

    // Update user information
    public async Task<BonDomainResult> UpdateAsync(TUser entity)
    {
        try
        {
            await UserRepository.UpdateAsync(entity, true);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error updating user.");
            return BonDomainResult.Failure("Error updating user.");
        }
    }
    // Find user by username
    public async Task<BonDomainResult<TUser>> FindByIdAsync(BonUserId id)
    {
        try
        {
            var user = await UserRepository.FindOneAsync(x => x.Id == id);
            if (user == null)
            {
                return BonDomainResult<TUser>.Failure($"User with username {id} not found.");
            }

            return BonDomainResult<TUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by username.");
            return BonDomainResult<TUser>.Failure("Error finding user by username.");
        }
    }
    // Find user by username
    public async Task<BonDomainResult<TUser>> FindByUserNameAsync(string userName)
    {
        try
        {
            var user = await UserRepository.FindOneAsync(x => x.UserName.Equals(userName));
            if (user == null)
            {
                return BonDomainResult<TUser>.Failure($"User with username {userName} not found.");
            }

            return BonDomainResult<TUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by username.");
            return BonDomainResult<TUser>.Failure("Error finding user by username.");
        }
    }

    // Find user by phone number (string)
    public async Task<BonDomainResult<TUser>> FindByPhoneNumberAsync(string phoneNumber)
    {
        try
        {
            var user = await UserRepository.FindOneAsync(x =>
                x.PhoneNumber != null && x.PhoneNumber.Number.Equals(phoneNumber));
            if (user == null)
            {
                return BonDomainResult<TUser>.Failure($"User with phone number {phoneNumber} not found.");
            }

            return BonDomainResult<TUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by phone number.");
            return BonDomainResult<TUser>.Failure("Error finding user by phone number.");
        }
    }

    // Find user by phone number (value object)
    public async Task<BonDomainResult<TUser>> FindByPhoneNumberAsync(BonUserPhoneNumber phoneNumber)
    {
        try
        {
            var user = await UserRepository.FindOneAsync(
                x => x.PhoneNumber != null && x.PhoneNumber.Equals(phoneNumber));
            if (user == null)
            {
                return BonDomainResult<TUser>.Failure($"User with phone number {phoneNumber} not found.");
            }

            return BonDomainResult<TUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by phone number.");
            return BonDomainResult<TUser>.Failure("Error finding user by phone number.");
        }
    }

    // Find user by email (string)
    public async Task<BonDomainResult<TUser>> FindByEmailAsync(string email)
    {
        try
        {
            var user = await UserRepository.FindOneAsync(x => x.Email != null && x.Email.Address.Equals(email));
            if (user == null)
            {
                return BonDomainResult<TUser>.Failure($"User with email {email} not found.");
            }

            return BonDomainResult<TUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by email.");
            return BonDomainResult<TUser>.Failure("Error finding user by email.");
        }
    }

    // Find user by email (value object)
    public async Task<BonDomainResult<TUser>> FindByEmailAsync(BonUserEmail email)
    {
        try
        {
            var user = await UserRepository.FindOneAsync(x => x.Email != null && x.Email.Equals(email));
            if (user == null)
            {
                return BonDomainResult<TUser>.Failure($"User with email {email} not found.");
            }

            return BonDomainResult<TUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by email.");
            return BonDomainResult<TUser>.Failure("Error finding user by email.");
        }
    }

    // Verify email
    public async Task<BonDomainResult> VerifyEmailAsync(TUser user)
    {
        try
        {
            if (user.Email == null)
            {
                return BonDomainResult.Failure("Email address is not set.");
            }

            user.VerifyEmail();
            await UpdateAsync(user);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error verifying email.");
            return BonDomainResult.Failure("Error verifying email.");
        }
    }

    // Verify phone number
    public async Task<BonDomainResult> VerifyPhoneNumberAsync(TUser user)
    {
        try
        {
            if (user.PhoneNumber == null)
            {
                return BonDomainResult.Failure("Phone number is not set.");
            }

            user.VerifyPhoneNumber();
            await UpdateAsync(user);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error verifying phone number.");
            return BonDomainResult.Failure("Error verifying phone number.");
        }
    }

    // Activate user
    public async Task<BonDomainResult> ActivateUserAsync(TUser user)
    {
        return await ChangeUserStatusAsync(user, UserStatus.Active);
    }

    // Deactivate user
    public async Task<BonDomainResult> DeactivateUserAsync(TUser user)
    {
        return await ChangeUserStatusAsync(user, UserStatus.Deactivated);
    }

    public async Task<BonDomainResult> SuspendUserAsync(TUser user)
    {
        return await ChangeUserStatusAsync(user, UserStatus.Suspended);
    }


    // Change user status
    public async Task<BonDomainResult> ChangeUserStatusAsync(TUser user, UserStatus newStatus)
    {
        try
        {
            user.ChangeStatus(newStatus);
            await UpdateAsync(user);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error changing user status.");
            return BonDomainResult.Failure("Error changing user status.");
        }
    }


}

