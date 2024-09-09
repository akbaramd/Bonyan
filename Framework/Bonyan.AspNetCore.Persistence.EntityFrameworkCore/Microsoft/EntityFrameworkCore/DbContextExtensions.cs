using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Provides extension methods for bulk operations on a DbContext.
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Performs a bulk insert operation asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entities.</typeparam>
        /// <param name="context">The DbContext instance.</param>
        /// <param name="entities">The collection of entities to insert.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task BulkInsertAsync<T>(this DbContext context, IEnumerable<T> entities) where T : class
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            await context.Set<T>().AddRangeAsync(entities);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Performs a bulk update operation asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entities.</typeparam>
        /// <param name="context">The DbContext instance.</param>
        /// <param name="entities">The collection of entities to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task BulkUpdateAsync<T>(this DbContext context, IEnumerable<T> entities) where T : class
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            context.Set<T>().UpdateRange(entities);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Performs a bulk delete operation asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entities.</typeparam>
        /// <param name="context">The DbContext instance.</param>
        /// <param name="entities">The collection of entities to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task BulkDeleteAsync<T>(this DbContext context, IEnumerable<T> entities) where T : class
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            context.Set<T>().RemoveRange(entities);
            await context.SaveChangesAsync();
        }
    }
}
