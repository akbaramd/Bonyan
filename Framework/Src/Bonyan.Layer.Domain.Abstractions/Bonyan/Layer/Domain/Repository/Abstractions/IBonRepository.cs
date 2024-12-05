    using Bonyan.Layer.Domain.Entity;
    using Bonyan.MultiTenant;
    using Bonyan.UnitOfWork;

    namespace Bonyan.Layer.Domain.Repository.Abstractions;

    public interface IBonRepository 
    {
        bool? IsChangeTrackingEnabled { get; }
        public IBonCurrentTenant? CurrentTenant { get; }
    }

    // IReadOnlyRepository without specifying the key type (TKesdasdasy is assumed internally by the entity).

    // IRepository for full CRUD operations, extending IReadOnlyRepository.
    public interface IBonRepository<TEntity, in TKey> : IBonRepository<TEntity>, IBonReadOnlyRepository<TEntity, TKey>
        where TEntity : class, IBonEntity<TKey>
    {
        Task DeleteByIdAsync(TKey id, bool autoSave = false);
    }

    // IRepository without specifying the key type (uses IEntity directly).
    public interface IBonRepository<TEntity> : IBonReadOnlyRepository<TEntity>, IBonUnitOfWorkEnabled where TEntity : class, IBonEntity
    {
        Task<TEntity> AddAsync(TEntity entity, bool autoSave = false);
        Task UpdateAsync(TEntity entity, bool autoSave = false);
        Task DeleteAsync(TEntity entity, bool autoSave = false);
        
        // Bulk operations
        Task AddRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false);
        Task UpdateRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false);
        Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false);
    }