using Bonyan.IdentityManagement.Domain;
using Bonyan.UnitOfWork;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Bonyan.UserManagement;

public class BonRoleStore<TRole> : IQueryableRoleStore<TRole>, IRoleStore<TRole>
    where TRole : BonRole
{
    private readonly IBonRoleRepository<TRole> _userRepository;

    public BonRoleStore(IBonRoleRepository<TRole> userRepository)
    {
        _userRepository = userRepository;
    }

    public void Dispose()
    {
    }


    public IQueryable<TRole> Roles => _userRepository.Queryable;

    [UnitOfWork]
    public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
    {
        await _userRepository.DeleteAsync(role);
        return IdentityResult.Success;
    }

    [UnitOfWork]
    public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
    {
        await _userRepository.UpdateAsync(role);
        return IdentityResult.Success;
    }

    [UnitOfWork]
    public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
    {
        await _userRepository.DeleteAsync(role);
        return IdentityResult.Success;
    }

    public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Id.Value.ToString());
    }

    public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Name);
    }

    public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
    {
        role.Name = roleName.ToLower();
        return Task.FromResult(0);
    }

    public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Name);
    }

    public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
    {
        role.Name = normalizedName.ToLower();
        return Task.CompletedTask;
    }

    [UnitOfWork]
    public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        return _userRepository.GetOneAsync(x => x.Id == RoleId.FromString(roleId));
    }

    [UnitOfWork]
    public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        return _userRepository.GetOneAsync(x => x.Name.ToLower().Equals(normalizedRoleName.ToLower()));
    }
}