using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.Repositories;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.DomainService;

namespace Bonyan.IdentityManagement.Domain.Roles.DomainServices;

/// <summary>
/// Domain service for identity roles (non-generic).
/// </summary>
public class BonIdentityRoleManager : IBonIdentityRoleManager
{
    private readonly IBonIdentityRoleRepository _roleRepository;

    public BonIdentityRoleManager(IBonIdentityRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<BonDomainResult> CreateRoleAsync(BonIdentityRole role)
    {
        if (await _roleRepository.ExistsAsync(x => x.Id == role.Id))
            return BonDomainResult.Failure("Role already exists.");
        await _roleRepository.AddAsync(role, true);
        return BonDomainResult.Success();
    }

    public async Task<BonDomainResult> UpdateRoleAsync(BonIdentityRole role)
    {
        if (!await _roleRepository.ExistsAsync(x => x.Id == role.Id))
            return BonDomainResult.Failure("Role does not exist.");
        await _roleRepository.UpdateAsync(role, true);
        return BonDomainResult.Success();
    }

    public async Task<BonDomainResult> DeleteRoleAsync(BonIdentityRole role)
    {
        if (!await _roleRepository.ExistsAsync(x => x.Id == role.Id))
            return BonDomainResult.Failure("Role does not exist.");
        role.Delete();
        await _roleRepository.DeleteAsync(role, true);
        return BonDomainResult.Success();
    }

    public async Task<BonDomainResult> CreateRoleWithClaimsAsync(BonIdentityRole role, IEnumerable<(string claimType, string claimValue, string? issuer)> claims)
    {
        var createRoleResult = await CreateRoleAsync(role);
        if (!createRoleResult.IsSuccess) return createRoleResult;
        foreach (var (claimType, claimValue, issuer) in claims)
        {
            var addClaimResult = await AddClaimToRoleAsync(role, claimType, claimValue, issuer);
            if (!addClaimResult.IsSuccess) return addClaimResult;
        }
        return BonDomainResult.Success();
    }

    public async Task<BonDomainResult> AddClaimToRoleAsync(BonIdentityRole role, string claimType, string claimValue, string? issuer = null)
    {
        if (string.IsNullOrWhiteSpace(claimType)) return BonDomainResult.Failure("Claim type cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(claimValue)) return BonDomainResult.Failure("Claim value cannot be null or empty.");
        if (role.HasClaim(claimType, claimValue)) return BonDomainResult.Failure($"Role already has claim '{claimType}' with value '{claimValue}'.");
        try
        {
            role.AddClaim(BonRoleClaimId.NewId(), claimType, claimValue, issuer);
            await _roleRepository.UpdateAsync(role, true);
            return BonDomainResult.Success();
        }
        catch (Exception ex)
        {
            return BonDomainResult.Failure($"Error adding claim: {ex.Message}");
        }
    }

    public async Task<BonDomainResult> RemoveClaimFromRoleAsync(BonIdentityRole role, string claimType, string claimValue)
    {
        if (string.IsNullOrWhiteSpace(claimType)) return BonDomainResult.Failure("Claim type cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(claimValue)) return BonDomainResult.Failure("Claim value cannot be null or empty.");
        if (!role.HasClaim(claimType, claimValue)) return BonDomainResult.Failure($"Role does not have claim '{claimType}' with value '{claimValue}'.");
        try
        {
            role.RemoveClaim(claimType, claimValue);
            await _roleRepository.UpdateAsync(role, true);
            return BonDomainResult.Success();
        }
        catch (Exception ex)
        {
            return BonDomainResult.Failure($"Error removing claim: {ex.Message}");
        }
    }

    public async Task<BonDomainResult> RemoveClaimsByTypeFromRoleAsync(BonIdentityRole role, string claimType)
    {
        if (string.IsNullOrWhiteSpace(claimType)) return BonDomainResult.Failure("Claim type cannot be null or empty.");
        try
        {
            role.RemoveClaimsByType(claimType);
            await _roleRepository.UpdateAsync(role, true);
            return BonDomainResult.Success();
        }
        catch (Exception ex)
        {
            return BonDomainResult.Failure($"Error removing claims by type: {ex.Message}");
        }
    }

    public Task<BonDomainResult<bool>> HasClaimAsync(BonIdentityRole role, string claimType, string claimValue)
    {
        if (string.IsNullOrWhiteSpace(claimType)) return Task.FromResult(BonDomainResult<bool>.Failure("Claim type cannot be null or empty."));
        if (string.IsNullOrWhiteSpace(claimValue)) return Task.FromResult(BonDomainResult<bool>.Failure("Claim value cannot be null or empty."));
        try
        {
            return Task.FromResult(BonDomainResult<bool>.Success(role.HasClaim(claimType, claimValue)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(BonDomainResult<bool>.Failure($"Error checking claim: {ex.Message}"));
        }
    }

    public Task<BonDomainResult<IEnumerable<BonIdentityRoleClaims>>> GetClaimsByTypeAsync(BonIdentityRole role, string claimType)
    {
        if (string.IsNullOrWhiteSpace(claimType)) return Task.FromResult(BonDomainResult<IEnumerable<BonIdentityRoleClaims>>.Failure("Claim type cannot be null or empty."));
        try
        {
            var claims = role.GetClaimsByType(claimType);
            return Task.FromResult(BonDomainResult<IEnumerable<BonIdentityRoleClaims>>.Success(claims));
        }
        catch (Exception ex)
        {
            return Task.FromResult(BonDomainResult<IEnumerable<BonIdentityRoleClaims>>.Failure($"Error getting claims by type: {ex.Message}"));
        }
    }

    public async Task<BonDomainResult<BonIdentityRole>> FindRoleByIdAsync(string roleKey)
    {
        var role = await _roleRepository.FindOneAsync(x => x.Id == BonRoleId.NewId(roleKey));
        if (role == null) return BonDomainResult<BonIdentityRole>.Failure("Role not found.");
        return BonDomainResult<BonIdentityRole>.Success(role);
    }

    public async Task<BonDomainResult<IEnumerable<BonIdentityRole>>> GetAllRolesAsync()
    {
        try
        {
            var roles = await _roleRepository.FindAsync(x => true);
            return BonDomainResult<IEnumerable<BonIdentityRole>>.Success(roles);
        }
        catch (Exception ex)
        {
            return BonDomainResult<IEnumerable<BonIdentityRole>>.Failure($"Error retrieving all roles: {ex.Message}");
        }
    }
}
