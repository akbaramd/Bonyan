using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Aggregate;
using System;

namespace Bonyan.IdentityManagement.Domain.Roles
{
   /// <summary>
    /// Represents an identity role in the domain.
    /// Encapsulates role behavior and adheres to DDD principles.
    /// </summary>
    public class BonIdentityRole<TRole> : BonAggregateRoot<BonRoleId> where TRole : BonIdentityRole<TRole>
    {
        private readonly List<BonIdentityRoleClaims<TRole>> _claims = new List<BonIdentityRoleClaims<TRole>>();

        // Private constructor for ORM or factory use
        protected BonIdentityRole()
        {
        }

        // Main constructor to initialize essential properties
        public BonIdentityRole(BonRoleId id, string title, bool canBeDeleted = true)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            SetTitle(title);
            CanBeDeleted = canBeDeleted;
        }

        /// <summary>
        /// Gets the title of the role.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Indicates whether the role can be deleted.
        /// </summary>
        public bool CanBeDeleted { get; private set; } = true;

    
        /// <summary>
        /// Gets the claims associated with the role.
        /// </summary>
        public IReadOnlyCollection<BonIdentityRoleClaims<TRole>> RoleClaims => _claims;



        /// <summary>
        /// Updates the title of the role.
        /// </summary>
        /// <param name="newTitle">The new title for the role.</param>
        /// <exception cref="ArgumentException">Thrown if the title is null or empty.</exception>
        public void UpdateTitle(string newTitle)
        {
            SetTitle(newTitle);
        }

       
        /// <summary>
        /// Adds a claim to the role.
        /// </summary>
        /// <param name="claimId">The unique identifier of the claim.</param>
        /// <param name="claimType">The type of the claim.</param>
        /// <param name="claimValue">The value of the claim.</param>
        /// <param name="issuer">The issuer of the claim (optional).</param>
        public void AddClaim(BonRoleClaimId claimId, string claimType, string claimValue, string? issuer = null)
        {
            if (claimId == null)
                throw new ArgumentNullException(nameof(claimId));
            if (string.IsNullOrEmpty(claimType))
                throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));
            if (string.IsNullOrEmpty(claimValue))
                throw new ArgumentException("Claim value cannot be null or empty.", nameof(claimValue));

            var existingClaim = _claims.FirstOrDefault(c => c.ClaimType == claimType && c.ClaimValue == claimValue);
            if (existingClaim == null)
            {
                var claim = new BonIdentityRoleClaims<TRole>(claimId, Id, claimType, claimValue, issuer);
                _claims.Add(claim);
            }
        }

        /// <summary>
        /// Removes a claim from the role.
        /// </summary>
        /// <param name="claimType">The type of the claim to remove.</param>
        /// <param name="claimValue">The value of the claim to remove.</param>
        public void RemoveClaim(string claimType, string claimValue)
        {
            if (string.IsNullOrEmpty(claimType))
                throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));
            if (string.IsNullOrEmpty(claimValue))
                throw new ArgumentException("Claim value cannot be null or empty.", nameof(claimValue));

            var roleClaim = _claims.FirstOrDefault(c => c.ClaimType == claimType && c.ClaimValue == claimValue);
            if (roleClaim != null)
            {
                _claims.Remove(roleClaim);
            }
        }

        /// <summary>
        /// Removes all claims of a specific type from the role.
        /// </summary>
        /// <param name="claimType">The type of claims to remove.</param>
        public void RemoveClaimsByType(string claimType)
        {
            if (string.IsNullOrEmpty(claimType))
                throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));

            var claimsToRemove = _claims.Where(c => c.ClaimType == claimType).ToList();
            foreach (var claim in claimsToRemove)
            {
                _claims.Remove(claim);
            }
        }

        /// <summary>
        /// Checks if the role has a specific claim.
        /// </summary>
        /// <param name="claimType">The type of the claim.</param>
        /// <param name="claimValue">The value of the claim.</param>
        /// <returns>True if the role has the claim, otherwise false.</returns>
        public bool HasClaim(string claimType, string claimValue)
        {
            if (string.IsNullOrEmpty(claimType))
                throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));
            if (string.IsNullOrEmpty(claimValue))
                throw new ArgumentException("Claim value cannot be null or empty.", nameof(claimValue));

            return _claims.Any(c => c.ClaimType == claimType && c.ClaimValue == claimValue);
        }

        /// <summary>
        /// Gets all claims of a specific type.
        /// </summary>
        /// <param name="claimType">The type of claims to retrieve.</param>
        /// <returns>A collection of claims with the specified type.</returns>
        public IEnumerable<BonIdentityRoleClaims<TRole>> GetClaimsByType(string claimType)
        {
            if (string.IsNullOrEmpty(claimType))
                throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));

            return _claims.Where(c => c.ClaimType == claimType);
        }

        /// <summary>
        /// Marks the role as non-deletable.
        /// </summary>
        public void MarkAsNonDeletable()
        {
            CanBeDeleted = false;
        }

        /// <summary>
        /// Marks the role as deletable.
        /// </summary>
        public void MarkAsDeletable()
        {
            CanBeDeleted = true;
        }

        /// <summary>
        /// Deletes the role if it is deletable.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the role is non-deletable.</exception>
        public void Delete()
        {
            if (!CanBeDeleted)
                throw new InvalidOperationException("This role cannot be deleted because it is marked as non-deletable.");

            // Domain event for deletion can be added here if necessary
            // AddDomainEvent(new RoleDeletedDomainEvent(Id));
        }

        /// <summary>
        /// Sets the title of the role with validation.
        /// </summary>
        /// <param name="title">The title to set.</param>
        /// <exception cref="ArgumentException">Thrown if the title is null or empty.</exception>
        public void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));

            Title = title;
        }
    }

}
