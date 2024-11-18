using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Repository.Abstractions;
using Bonyan.UserManagement.Application.Users.Dto;
using Bonyan.UserManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.UserManagement.Application.Users.Services;

public class BonUserCrudAppService<TUser> : BonCrudAppService<TUser, BonUserId, BonUserFilterAndPaginateDto, BonUserDto,
        BonUserDto, BonUserCreateDto, BonUserUpdateDto>,
    IBonUserCrudAppService where TUser : class, IBonUser
{
    public BonUserCrudAppService(IBonRepository<TUser, BonUserId> repository) : base(repository)
    {
    }

    protected override IQueryable<TUser> ApplyFiltering(IQueryable<TUser> query, BonUserFilterAndPaginateDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Search))
        {
            return query;
        }

        var searchLower = input.Search.ToLower();

        return query.Where(x =>
            x.UserName.Contains(searchLower, StringComparison.CurrentCultureIgnoreCase) ||
            (x.PhoneNumber != null &&
             x.PhoneNumber.Number.Contains(searchLower, StringComparison.CurrentCultureIgnoreCase)) ||
            (x.Email != null && x.Email.Address.Contains(searchLower, StringComparison.CurrentCultureIgnoreCase)));
    }
}