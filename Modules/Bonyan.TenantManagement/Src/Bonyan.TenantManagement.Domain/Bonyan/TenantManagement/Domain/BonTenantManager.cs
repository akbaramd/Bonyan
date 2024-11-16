using Bonyan.Core;
using Bonyan.Exceptions;
using Bonyan.Layer.Domain.Services;
using Bonyan.TenantManagement.Domain.Events;

namespace Bonyan.TenantManagement.Domain
{
    /// <summary>
    /// Manages tenant operations, including creation, deletion, and updating tenant names.
    /// </summary>
    public class BonTenantManager : BonDomainService, IBonTenantManager
    {
        /// <summary>
        /// The repository for tenant-related data operations.
        /// </summary>
        private IBonTenantRepository BonTenantRepository => LazyServiceProvider.LazyGetRequiredService<IBonTenantRepository>();

        /// <summary>
        /// The event publisher used to publish domain events.
        /// </summary>

        /// <summary>
        /// Creates a new tenant with the specified key.
        /// </summary>
        /// <param name="key">The unique key for the new tenant.</param>
        /// <returns>The newly created <see cref="BonTenant"/> instance.</returns>
        public virtual async Task<BonTenant> CreateAsync(string key)
        {
            Check.NotNull(key, nameof(key));

            // Validate the key to ensure no duplicate tenants exist
            await ValidateNameAsync(key);
            return new BonTenant(key);
        }

        /// <summary>
        /// Deletes a tenant with the specified key and publishes a <see cref="TenantDeletedEvent"/> on successful deletion.
        /// </summary>
        /// <param name="key">The unique key of the tenant to delete.</param>
        /// <returns>The deleted <see cref="BonTenant"/> instance.</returns>
        /// <exception cref="BusinessException">Thrown if the tenant with the specified key is not found.</exception>
        public async Task<BonTenant> DeleteAsync(string key)
        {
            Check.NotNull(key, nameof(key));

            // Find the tenant to delete
            var tenant = await BonTenantRepository.FindByKeyAsync(key);
            if (tenant == null)
            {
                throw new BusinessException("Bonyan:TenantManagement:TenantNotFound").WithData("Key", key);
            }

            // Delete the tenant from the repository
            await BonTenantRepository.DeleteAsync(tenant);

            // Publish the TenantDeletedEvent after successful deletion
            if (DomainEventDispatcher != null)
            {
                await DomainEventDispatcher.DispatchAsync(new TenantDeletedBonDomainEvent(tenant.Id));
            }

            return tenant;
        }

        /// <summary>
        /// Changes the key of an existing tenant after validating that the new key is unique.
        /// </summary>
        /// <param name="bonTenant">The tenant instance to update.</param>
        /// <param name="key">The new unique key for the tenant.</param>
        /// <exception cref="BusinessException">Thrown if the new key is not unique.</exception>
        public virtual async Task ChangeNameAsync(BonTenant bonTenant, string key)
        {
            Check.NotNull(bonTenant, nameof(bonTenant));
            Check.NotNull(key, nameof(key));

            // Validate the new key for uniqueness
            await ValidateNameAsync(key);
            bonTenant.SetKey(key);
        }

        /// <summary>
        /// Validates that the specified key is unique and not already assigned to another tenant.
        /// </summary>
        /// <param name="key">The unique key to validate.</param>
        /// <param name="expectedId">The expected tenant ID (used when renaming an existing tenant).</param>
        /// <exception cref="BusinessException">Thrown if the key is already in use by another tenant.</exception>
        protected virtual async Task ValidateNameAsync(string key, BonTenantId? expectedId = null)
        {
            // Check for an existing tenant with the same key
            var tenant = await BonTenantRepository.FindByKeyAsync(key);
            if (tenant != null && tenant.Id != expectedId)
            {
                throw new BusinessException("Bonyan:TenantManagement:DuplicateTenantName").WithData("Key", key);
            }
        }
    }
}
