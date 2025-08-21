using Bonyan.IdentityManagement.Domain.Roles.Repostories;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.DomainService;

namespace Bonyan.IdentityManagement.Domain.Roles.DomainServices
{
    public class BonIdentityRoleManager<TRole> : IBonIdentityRoleManager<TRole> 
        where TRole : BonIdentityRole<TRole>
    {
        private readonly IBonIdentityRoleRepository<TRole> _roleRepository;
        private readonly IBonIdentityRoleClaimsRepository<TRole> _roleClaimsRepository;

        public BonIdentityRoleManager(
            IBonIdentityRoleRepository<TRole> roleRepository,
            IBonIdentityRoleClaimsRepository<TRole> roleClaimsRepository)
        {
            _roleRepository = roleRepository;
            _roleClaimsRepository = roleClaimsRepository;
        }

        /// <summary>
        /// Creates a role.
        /// </summary>
        public async Task<BonDomainResult> CreateRoleAsync(TRole role)
        {
            if (await _roleRepository.ExistsAsync(x => x.Id == role.Id))
            {
                return BonDomainResult.Failure("Role already exists.");
            }

            await _roleRepository.AddAsync(role, true);
            return BonDomainResult.Success();
        }

        /// <summary>
        /// Updates an existing role.
        /// </summary>
        public async Task<BonDomainResult> UpdateRoleAsync(TRole role)
        {
            if (!await _roleRepository.ExistsAsync(x => x.Id == role.Id))
            {
                return BonDomainResult.Failure("Role does not exist.");
            }

            await _roleRepository.UpdateAsync(role, true);
            return BonDomainResult.Success();
        }

        /// <summary>
        /// Deletes a role.
        /// </summary>
        public async Task<BonDomainResult> DeleteRoleAsync(TRole role)
        {
            if (!await _roleRepository.ExistsAsync(x => x.Id == role.Id))
            {
                return BonDomainResult.Failure("Role does not exist.");
            }

            role.Delete(); // Domain behavior ensures the role can be deleted
            await _roleRepository.DeleteAsync(role, true);

            return BonDomainResult.Success();
        }

        /// <summary>
        /// Creates a role with associated claims.
        /// </summary>
        public async Task<BonDomainResult> CreateRoleWithClaimsAsync(TRole role, IEnumerable<(string claimType, string claimValue, string? issuer)> claims)
        {
            var createRoleResult = await CreateRoleAsync(role);
            if (!createRoleResult.IsSuccess)
            {
                return createRoleResult;
            }

            foreach (var (claimType, claimValue, issuer) in claims)
            {
                var addClaimResult = await AddClaimToRoleAsync(role, claimType, claimValue, issuer);
                if (!addClaimResult.IsSuccess)
                {
                    return addClaimResult;
                }
            }

            return BonDomainResult.Success();
        }

        /// <summary>
        /// Adds a claim to a role using the role's domain behavior.
        /// </summary>
        public async Task<BonDomainResult> AddClaimToRoleAsync(TRole role, string claimType, string claimValue, string? issuer = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(claimType))
                {
                    return BonDomainResult.Failure("Claim type cannot be null or empty.");
                }

                if (string.IsNullOrWhiteSpace(claimValue))
                {
                    return BonDomainResult.Failure("Claim value cannot be null or empty.");
                }

                // Check if role already has this claim
                if (role.HasClaim(claimType, claimValue))
                {
                    return BonDomainResult.Failure($"Role already has claim '{claimType}' with value '{claimValue}'.");
                }

                // Use domain behavior to add claim
                role.AddClaim(BonRoleClaimId.NewId(), claimType, claimValue, issuer);

                // Persist changes
                await _roleRepository.UpdateAsync(role, true);
                return BonDomainResult.Success();
            }
            catch (Exception ex)
            {
                return BonDomainResult.Failure($"Error adding claim: {ex.Message}");
            }
        }

        /// <summary>
        /// Removes a claim from a role.
        /// </summary>
        public async Task<BonDomainResult> RemoveClaimFromRoleAsync(TRole role, string claimType, string claimValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(claimType))
                {
                    return BonDomainResult.Failure("Claim type cannot be null or empty.");
                }

                if (string.IsNullOrWhiteSpace(claimValue))
                {
                    return BonDomainResult.Failure("Claim value cannot be null or empty.");
                }

                // Check if role has this claim
                if (!role.HasClaim(claimType, claimValue))
                {
                    return BonDomainResult.Failure($"Role does not have claim '{claimType}' with value '{claimValue}'.");
                }

                // Use domain behavior to remove claim
                role.RemoveClaim(claimType, claimValue);

                // Persist changes
                await _roleRepository.UpdateAsync(role, true);
                return BonDomainResult.Success();
            }
            catch (Exception ex)
            {
                return BonDomainResult.Failure($"Error removing claim: {ex.Message}");
            }
        }

        /// <summary>
        /// Removes all claims of a specific type from a role.
        /// </summary>
        public async Task<BonDomainResult> RemoveClaimsByTypeFromRoleAsync(TRole role, string claimType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(claimType))
                {
                    return BonDomainResult.Failure("Claim type cannot be null or empty.");
                }

                // Use domain behavior to remove claims by type
                role.RemoveClaimsByType(claimType);

                // Persist changes
                await _roleRepository.UpdateAsync(role, true);
                return BonDomainResult.Success();
            }
            catch (Exception ex)
            {
                return BonDomainResult.Failure($"Error removing claims by type: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a role has a specific claim.
        /// </summary>
        public async Task<BonDomainResult<bool>> HasClaimAsync(TRole role, string claimType, string claimValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(claimType))
                {
                    return BonDomainResult<bool>.Failure("Claim type cannot be null or empty.");
                }

                if (string.IsNullOrWhiteSpace(claimValue))
                {
                    return BonDomainResult<bool>.Failure("Claim value cannot be null or empty.");
                }

                var hasClaim = role.HasClaim(claimType, claimValue);
                return BonDomainResult<bool>.Success(hasClaim);
            }
            catch (Exception ex)
            {
                return BonDomainResult<bool>.Failure($"Error checking claim: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all claims of a specific type for a role.
        /// </summary>
        public async Task<BonDomainResult<IEnumerable<BonIdentityRoleClaims<TRole>>>> GetClaimsByTypeAsync(TRole role, string claimType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(claimType))
                {
                    return BonDomainResult<IEnumerable<BonIdentityRoleClaims<TRole>>>.Failure("Claim type cannot be null or empty.");
                }

                var claims = role.GetClaimsByType(claimType);
                return BonDomainResult<IEnumerable<BonIdentityRoleClaims<TRole>>>.Success(claims);
            }
            catch (Exception ex)
            {
                return BonDomainResult<IEnumerable<BonIdentityRoleClaims<TRole>>>.Failure($"Error getting claims by type: {ex.Message}");
            }
        }

        /// <summary>
        /// Finds a role by its ID.
        /// </summary>
        public async Task<BonDomainResult<TRole>> FindRoleByIdAsync(string roleKey)
        {
            var role = await _roleRepository.FindOneAsync(x => x.Id == BonRoleId.NewId(roleKey));
            if (role == null)
            {
                return BonDomainResult<TRole>.Failure("Role not found.");
            }

            return BonDomainResult<TRole>.Success(role);
        }

        public async Task<BonDomainResult<IEnumerable<TRole>>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _roleRepository.FindAsync(x=>true);
                return BonDomainResult<IEnumerable<TRole>>.Success(roles);
            }
            catch (Exception ex)
            {
                return BonDomainResult<IEnumerable<TRole>>.Failure($"Error retrieving all roles: {ex.Message}");
            }
        }
    }
}
