using Bonyan.AspNetCore.Persistence.EntityFrameworkCore.Abstractions;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.AspNetCore.Persistence.EntityFrameworkCore;

public class BonyanDbContext<TDbContext> : DbContext , IBonyanDbContext<TDbContext> where TDbContext: DbContext
{
  public BonyanDbContext(DbContextOptions<TDbContext> options):base(options)
  {
    
  }
  
  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
  {
    var entries = ChangeTracker.Entries()
      .Where(e => e.Entity is ICreationAuditable || e.Entity is IModificationAuditable || e.Entity is ISoftDeleteAuditable)
      .ToList();

    foreach (var entry in entries)
    {
      if (entry.State == EntityState.Added && entry.Entity is ICreationAuditable creationAuditableEntity)
      {
        creationAuditableEntity.CreatedDate = DateTime.UtcNow;
      }

      if (entry.State == EntityState.Modified)
      {
        if (entry.Entity is IModificationAuditable updateAuditableEntity)
        {
          updateAuditableEntity.ModifiedDate = DateTime.UtcNow;
        }

        if (entry.Entity is ISoftDeleteAuditable softDeletableEntity)
        {
          if (softDeletableEntity.IsDeleted)
          {
            softDeletableEntity.DeletedDate = DateTime.UtcNow;
          }
          else
          {
            entry.Property(nameof(ISoftDeleteAuditable.DeletedDate)).IsModified = false;
            entry.Property(nameof(ISoftDeleteAuditable.IsDeleted)).IsModified = false;
          }
        }
      }
    }

    return await base.SaveChangesAsync(cancellationToken);
  }

  public override int SaveChanges()
  {
    return SaveChangesAsync().GetAwaiter().GetResult();
  }
}
