using Bonyan.EntityFrameworkCore;
using Bonyan.Layer.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.Messaging.OutBox
{
    public class EfCoreOutboxStore<TDbContext> : IOutboxStore where TDbContext : IBonOutBoxDbContext
    {
        private readonly TDbContext _context;

        public EfCoreOutboxStore(TDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Adds a new outbox message to the store.
        /// </summary>
        public async Task AddAsync(BonOutboxMessage message, CancellationToken cancellationToken = default)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            await _context.OutboxMessages.AddAsync(message, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves all pending outbox messages.
        /// </summary>
        public async Task<IEnumerable<BonOutboxMessage>> GetPendingMessagesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.OutboxMessages
                .AsNoTracking()
                .OrderBy(m => m.CreatedDate) // Optional: Order by creation time
                .ToListAsync(cancellationToken);
        }


        /// <summary>
        /// Deletes an outbox message by its ID.
        /// </summary>
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var message = await _context.OutboxMessages
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

            if (message != null)
            {
                _context.OutboxMessages.Remove(message);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                throw new KeyNotFoundException($"Outbox message with ID {id} not found.");
            }
        }
    }
}
