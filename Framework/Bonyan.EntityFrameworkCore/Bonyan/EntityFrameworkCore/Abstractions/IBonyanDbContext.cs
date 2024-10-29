using Microsoft.EntityFrameworkCore;

namespace Bonyan.EntityFrameworkCore.Abstractions;

public interface IBonyanDbContext<TDbContext> : IEfCoreDbContext where TDbContext: DbContext
{
  
}
