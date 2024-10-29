using System.Linq.Expressions;
using System.Reflection;
using Bonyan.DependencyInjection;
using Bonyan.EntityFrameworkCore.Abstractions;
using Bonyan.Layer.Domain.Abstractions;
using Bonyan.Layer.Domain.Entities;
using Bonyan.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bonyan.EntityFrameworkCore;

public class BonyanDbContext<TDbContext> : DbContext , IBonyanDbContext<TDbContext> where TDbContext: DbContext
{
  
  public IBonyanLazyServiceProvider ServiceProvider { get; set; } = default!;
  
  public BonyanDbContext(DbContextOptions<TDbContext> options):base(options)
  {
    
  }
  
  
  public ICurrentTenant CurrentTenant => ServiceProvider.GetRequiredService<ICurrentTenant>();
  private BonyanMultiTenancyOptions TenancyOptions=> ServiceProvider.GetRequiredService<IOptions<BonyanMultiTenancyOptions>>().Value;
  protected virtual Guid? CurrentTenantId => CurrentTenant?.Id;


  private static readonly MethodInfo ConfigureBasePropertiesMethodInfo
    = typeof(BonyanDbContext<TDbContext>)
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
  
  
  
  protected virtual void ConfigureBaseProperties<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
    where TEntity : class
  {
    if (mutableEntityType.IsOwned())
    {
      return;
    }

    if (!typeof(IEntity).IsAssignableFrom(typeof(TEntity)))
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
    if (typeof(IMultiTenant).IsAssignableFrom(typeof(TEntity)))
    {
      return true;
    }

    if (typeof(ISoftDeleteAuditable).IsAssignableFrom(typeof(TEntity)))
    {
      return true;
    }

    return false;
  }

  protected virtual Expression<Func<TEntity, bool>>? CreateFilterExpression<TEntity>()
    where TEntity : class
  {
    Expression<Func<TEntity, bool>>? expression = null;

    if (typeof(ISoftDeleteAuditable).IsAssignableFrom(typeof(TEntity)))
    {
      expression = e =>  !EF.Property<bool>(e, "IsDeleted");
    }

    if (TenancyOptions.IsEnabled && typeof(IMultiTenant).IsAssignableFrom(typeof(TEntity)))
    {
      Expression<Func<TEntity, bool>> multiTenantFilter = e =>     EF.Property<Guid>(e, "TenantId") == CurrentTenantId;
      expression = expression == null ? multiTenantFilter : QueryFilterExpressionHelper.CombineExpressions(expression, multiTenantFilter);
    }

    return expression;
  }
}
