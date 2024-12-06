using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.IdentityManagement.Domain.Roles.DomainServices;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Repository.Abstractions;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Microsoft.Extensions.Logging;
// Add this

namespace Bonyan.IdentityManagement.Application.Users
{
    public class BonIdentityUserAppService<TUser> :
        BonApplicationService,
        IBonIdentityUserAppService where TUser : class, IBonIdentityUser
    {
        // We need to use both UserManager and RoleManager
        public IBonIdentityUserManager<TUser> UserManager => LazyServiceProvider.LazyGetRequiredService<IBonIdentityUserManager<TUser>>();
        public IBonIdentityRoleManager RoleManager => LazyServiceProvider.LazyGetRequiredService<IBonIdentityRoleManager>();  // New RoleManager

        // Add UserRepository
        private  IBonIdentityUserRepository<TUser> UserRepository => LazyServiceProvider.LazyGetRequiredService<IBonIdentityUserRepository<TUser>>();


        public async Task<ServiceResult<BonIdentityUserDto>> CreateAsync(BonIdentityUserCreateDto input)
        {
            var user = Mapper.Map<BonIdentityUserCreateDto,TUser>(input);

            // Create user and assign roles
            var createResult = await UserManager.CreateAsync(user, input.Password);
            if (!createResult.IsSuccess)
            {
                return ServiceResult<BonIdentityUserDto>.Failure(createResult.ErrorMessage);
            }

            var assignRolesResult = await UserManager.AssignRolesAsync(user, input.Roles.Select(x=>BonRoleId.NewId(x)));
            if (!assignRolesResult.IsSuccess)
            {
                return ServiceResult<BonIdentityUserDto>.Failure(assignRolesResult.ErrorMessage);
            }

            
            return await DetailAsync(user.Id);
        }

        public async Task<ServiceResult> DeleteAsync(BonUserId id)
        {
            var user = await UserRepository.FindOneAsync(x=>x.Id == id);
            if (user == null)
            {
                return ServiceResult.Failure("User not found");
            }

            await UserRepository.DeleteAsync(user,true);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult<BonIdentityUserDto>> DetailAsync(BonUserId key)
        {
            var user = await UserRepository.FindOneAsync(x=>x.Id == key);
            if (user == null)
            {
                return ServiceResult<BonIdentityUserDto>.Failure("User not found");
            }

            var userDto = Mapper.Map<BonIdentityUserDto>(user);
            return ServiceResult<BonIdentityUserDto>.Success(userDto);
        }

        public async Task<ServiceResult<BonPaginatedResult<BonIdentityUserDto>>> PaginatedAsync(BonFilterAndPaginateDto paginateDto)
        {
            var users = await UserRepository.PaginatedAsync(x=>paginateDto.Search == null || x.UserName.Contains(paginateDto.Search),paginateDto.Take,paginateDto.Skip);
            // map users and put in paginatedresults
            var userDtos = Mapper.Map<List<BonIdentityUserDto>>(users.Results);
            var res = new BonPaginatedResult<BonIdentityUserDto>(userDtos,paginateDto.Skip,paginateDto.Take,users.TotalCount);

            return ServiceResult<BonPaginatedResult<BonIdentityUserDto>>.Success(res);
        }

        public async Task<ServiceResult<BonIdentityUserDto>> UpdateAsync(BonUserId key, BonIdentityUserUpdateDto input)
        {
            var user = await UserRepository.FindOneAsync(x=>x.Id == key);
            if (user == null)
            {
                return ServiceResult<BonIdentityUserDto>.Failure("User not found");
            }

            // Update user properties from input
            Mapper.Map(input, user);

            // Assign new roles
            var assignRolesResult = await UserManager.AssignRolesAsync(user, input.Roles.Select(x=>BonRoleId.NewId(x)));
            if (!assignRolesResult.IsSuccess)
            {
                return ServiceResult<BonIdentityUserDto>.Failure(assignRolesResult.ErrorMessage);
            }

            return await DetailAsync(user.Id);
        }
    }
}
