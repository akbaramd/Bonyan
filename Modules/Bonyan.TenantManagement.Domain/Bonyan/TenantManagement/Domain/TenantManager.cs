using Bonyan.Core;
using Bonyan.Exceptions;
using Bonyan.Layer.Domain.Services;
using Bonyan.TenantManagement.Domain.Events;
using System.Threading.Tasks;
using Bonyan.Layer.Domain.Events;

namespace Bonyan.TenantManagement.Domain
{
    /// <summary>
    /// Manages tenant operations, including creation, deletion, and updating tenant names.
    /// </summary>
    public class TenantManager : DomainService, ITenantManager
    {
        /// <summary>
        /// The repository for tenant-related data operations.
        /// </summary>
        private ITenantRepository TenantRepository => LazyServiceProvider.LazyGetRequiredService<ITenantRepository>();

        /// <summary>
        /// The event publisher used to publish domain events.
        /// </summary>

        /// <summary>
        /// Creates a new tenant with the specified key.
        /// </summary>
        /// <param name="key">The unique key for the new tenant.</param>
        /// <returns>The newly created <see cref="Tenant"/> instance.</returns>
        public virtual async Task<Tenant> CreateAsync(string key)
        {
            Check.NotNull(key, nameof(key));

            // Validate the key to ensure no duplicate tenants exist
            await ValidateNameAsync(key);
            return new Tenant(key);
        }

        /// <summary>
        /// Deletes a tenant with the specified key and publishes a <see cref="TenantDeletedEvent"/> on successful deletion.
        /// </summary>
        /// <param name="key">The unique key of the tenant to delete.</param>
        /// <returns>The deleted <see cref="Tenant"/> instance.</returns>
        /// <exception cref="BusinessException">Thrown if the tenant with the specified key is not found.</exception>
        public async Task<Tenant> DeleteAsync(string key)
        {
            Check.NotNull(key, nameof(key));

            // Find the tenant to delete
            var tenant = await TenantRepository.FindByKeyAsync(key);
            if (tenant == null)
            {
                throw new BusinessException("Bonyan:TenantManagement:TenantNotFound").WithData("Key", key);
            }

            // Delete the tenant from the repository
            await TenantRepository.DeleteAsync(tenant);

            // Publish the TenantDeletedEvent after successful deletion
            if (DomainEventDispatcher != null)
            {
                await DomainEventDispatcher.DispatchAsync(new TenantDeletedDomainEvent(tenant.Id));
            }

            return tenant;
        }

        /// <summary>
        /// Changes the key of an existing tenant after validating that the new key is unique.
        /// </summary>
        /// <param name="tenant">The tenant instance to update.</param>
        /// <param name="key">The new unique key for the tenant.</param>
        /// <exception cref="BusinessException">Thrown if the new key is not unique.</exception>
        public virtual async Task ChangeNameAsync(Tenant tenant, string key)
        {
            Check.NotNull(tenant, nameof(tenant));
            Check.NotNull(key, nameof(key));

            // Validate the new key for uniqueness
            await ValidateNameAsync(key);
            tenant.SetKey(key);
        }

        /// <summary>
        /// Validates that the specified key is unique and not already assigned to another tenant.
        /// </summary>
        /// <param name="key">The unique key to validate.</param>
        /// <param name="expectedId">The expected tenant ID (used when renaming an existing tenant).</param>
        /// <exception cref="BusinessException">Thrown if the key is already in use by another tenant.</exception>
        protected virtual async Task ValidateNameAsync(string key, TenantId? expectedId = null)
        {
            // Check for an existing tenant with the same key
            var tenant = await TenantRepository.FindByKeyAsync(key);
            if (tenant != null && tenant.Id != expectedId)
            {
                throw new BusinessException("Bonyan:TenantManagement:DuplicateTenantName").WithData("Key", key);
            }
        }
    }
}
