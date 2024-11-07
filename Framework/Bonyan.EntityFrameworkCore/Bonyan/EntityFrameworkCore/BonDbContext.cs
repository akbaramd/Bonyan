using System.Linq.Expressions;
using System.Reflection;
using Bonyan.DependencyInjection;
using Bonyan.EntityFrameworkCore.Abstractions;
using Bonyan.Layer.Domain.Abstractions;
using Bonyan.Layer.Domain.Aggregates;
using Bonyan.Layer.Domain.Entities;
using Bonyan.Layer.Domain.Events;
using Bonyan.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bonyan.EntityFrameworkCore;

public class BonDbContext<TDbContext> : DbContext , IBonDbContext<TDbContext> where TDbContext: DbContext
{
  
  public IBonLazyServiceProvider? ServiceProvider { get; set; } = default!;
  
  public BonDbContext(DbContextOptions<TDbContext> options):base(options)
  {
    
  }
  
  
  public IBonCurrentTenant? CurrentTenant => ServiceProvider?.GetService<IBonCurrentTenant>();
  private IOptions<BonMultiTenancyOptions>? TenancyOptions => ServiceProvider?.GetService<IOptions<BonMultiTenancyOptions>>();
  private IBonDomainEventDispatcher? DomainEventDispatcher => ServiceProvider?.GetService<IBonDomainEventDispatcher>();
  protected virtual Guid? CurrentTenantId => CurrentTenant?.Id;


  private static readonly MethodInfo ConfigureBasePropertiesMethodInfo
    = typeof(BonDbContext<TDbContext>)
      .GetMethod(
        nameof(ConfigureBaseProperties),
        BindingFlags.Instance | BindingFlags.NonPublic
      )!;
  
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
      ConfigureBasePropertiesMethodInfo
        .MakeGenericMethod(entityType.ClrType)
        .Invoke(this, new object[] { modelBuilder, entityType });
    }
  }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
  {
    var entries = ChangeTracker.Entries()
      .Where(e => e.Entity is IBonCreationAuditable || e.Entity is IBonModificationAuditable || e.Entity is IBonSoftDeleteAuditable)
      .ToList();

    foreach (var entry in entries)
    {
      if (entry.State == EntityState.Added && entry.Entity is IBonCreationAuditable creationAuditableEntity)
      {
        creationAuditableEntity.CreatedDate = DateTime.UtcNow;
      }

      if (entry.State == EntityState.Modified)
      {
        if (entry.Entity is IBonModificationAuditable updateAuditableEntity)
        {
          updateAuditableEntity.ModifiedDate = DateTime.UtcNow;
        }

        if (entry.Entity is IBonSoftDeleteAuditable softDeletableEntity)
        {
          if (softDeletableEntity.IsDeleted)
          {
            softDeletableEntity.DeletedDate = DateTime.UtcNow;
          }
          else
          {
            entry.Property(nameof(IBonSoftDeleteAuditable.DeletedDate)).IsModified = false;
            entry.Property(nameof(IBonSoftDeleteAuditable.IsDeleted)).IsModified = false;
          }
        }
      }

      if (DomainEventDispatcher == null || entry.Entity is not IBonAggregateRoot agge) continue;
      
      foreach (var @event in agge.DomainEvents)
      {
        await DomainEventDispatcher.DispatchAsync(@event);
      }
    }

    
    
    return await base.SaveChangesAsync(cancellationToken);
  }

  public Task<int> SaveChangesOnDbContextAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
  {
    return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
  }

  
  protected virtual void ConfigureBaseProperties<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
    where TEntity : class
  {
    if (mutableEntityType.IsOwned())
    {
      return;
    }

    if (!typeof(IBonEntity).IsAssignableFrom(typeof(TEntity)))
    {
      return;
    }

    modelBuilder.Entity<TEntity>().ConfigureByConvention();

    ConfigureGlobalFilters<TEntity>(modelBuilder, mutableEntityType);
  }
  
  protected virtual void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
    where TEntity : class
  {
    if (mutableEntityType.BaseType == null && ShouldFilterEntity<TEntity>(mutableEntityType))
    {
      var filterExpression = CreateFilterExpression<TEntity>();
      if (filterExpression != null)
      {
        modelBuilder.Entity<TEntity>().HasBonyanQueryFilter(filterExpression);
      }
    }
  }
  
  protected virtual bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
  {
    if (typeof(IBonMultiTenant).IsAssignableFrom(typeof(TEntity)))
    {
      return true;
    }

    if (typeof(IBonSoftDeleteAuditable).IsAssignableFrom(typeof(TEntity)))
    {
      return true;
    }

    return false;
  }

  protected virtual Expression<Func<TEntity, bool>>? CreateFilterExpression<TEntity>()
    where TEntity : class
  {
    Expression<Func<TEntity, bool>>? expression = null;

    if (typeof(IBonSoftDeleteAuditable).IsAssignableFrom(typeof(TEntity)))
    {
      expression = e =>  !EF.Property<bool>(e, "IsDeleted");
    }
    
    if (TenancyOptions != null && TenancyOptions is { Value.IsEnabled: true } && typeof(IBonMultiTenant).IsAssignableFrom(typeof(TEntity)))
    {
      Expression<Func<TEntity, bool>> multiTenantFilter = e =>     EF.Property<Guid>(e, "TenantId") == CurrentTenantId;
      expression = expression == null ? multiTenantFilter : BonQueryFilterExpressionHelper.CombineExpressions(expression, multiTenantFilter);
    }

    return expression;
  }
}
