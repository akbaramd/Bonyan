using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.Persistence.EntityFrameworkCore;

public abstract class AuditableDbContext : DbContext
{
  protected AuditableDbContext(DbContextOptions options) : base(options) { }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
  {
    var entries = ChangeTracker.Entries()
      .Where(e => e.Entity is ICreationAuditable || e.Entity is IUpdateAuditable || e.Entity is ISoftDeletable)
      .ToList();

    foreach (var entry in entries)
    {
      if (entry.State == EntityState.Added && entry.Entity is ICreationAuditable creationAuditableEntity)
      {
        creationAuditableEntity.CreatedDate = DateTime.UtcNow;
      }

      if (entry.State == EntityState.Modified)
      {
        if (entry.Entity is IUpdateAuditable updateAuditableEntity)
        {
          updateAuditableEntity.UpdatedDate = DateTime.UtcNow;
        }

        if (entry.Entity is ISoftDeletable softDeletableEntity)
        {
          if (softDeletableEntity.IsDeleted)
          {
            softDeletableEntity.DeletedDate = DateTime.UtcNow;
          }
          else
          {
            entry.Property(nameof(ISoftDeletable.DeletedDate)).IsModified = false;
            entry.Property(nameof(ISoftDeletable.IsDeleted)).IsModified = false;
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