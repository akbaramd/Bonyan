using System.Linq.Expressions;
using Bonyan.IdentityManagement.Application.Users.Dtos;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Repository.Abstractions;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Microsoft.Extensions.Logging;

namespace Bonyan.IdentityManagement.Application.Users;

/// <summary>
/// Provides paginated user list for the Users index. Uses read-only repository.
/// </summary>
public class IdentityUserListAppService : BonApplicationService, IIdentityUserListAppService
{
    private readonly IBonIdentityUserReadOnlyRepository _userRepository;

    public IdentityUserListAppService(IBonIdentityUserReadOnlyRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ServiceResult<BonPaginatedResult<UserListDto>>> GetPaginatedAsync(UserFilterDto filter, CancellationToken cancellationToken = default)
    {
        if (filter == null)
            return ServiceResult<BonPaginatedResult<UserListDto>>.Failure("Filter cannot be null.", "NullInput");

        try
        {
            var search = filter.Search?.Trim().ToLowerInvariant();
            Expression<Func<BonIdentityUser, bool>> predicate = string.IsNullOrEmpty(search)
                ? _ => true
                : x => x.UserName != null && x.UserName.ToLower().Contains(search!);

            var paginated = await _userRepository.PaginatedAsync(predicate, filter.Take, filter.Skip);

            var dtos = paginated.Results.Select(MapToDto).ToList();
            var result = new BonPaginatedResult<UserListDto>(dtos, paginated.Skip, paginated.Take, paginated.TotalCount);
            return ServiceResult<BonPaginatedResult<UserListDto>>.Success(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading paginated user list.");
            return ServiceResult<BonPaginatedResult<UserListDto>>.Failure($"Error loading list: {ex.Message}", "PaginatedFailed");
        }
    }

    private static UserListDto MapToDto(BonIdentityUser user)
    {
        return new UserListDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email?.Address,
            PhoneNumber = user.PhoneNumber?.Number,
            Status = user.Status.ToString(),
            IsLocked = user.AccountLockedUntil.HasValue && user.AccountLockedUntil.Value > DateTime.UtcNow
        };
    }
}
