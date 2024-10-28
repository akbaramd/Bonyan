using Microsoft.EntityFrameworkCore;

namespace Bonyan.EntityFrameworkCore.Abstractions;

public interface IBonyanDbContext<TDbContext> : IDisposable where TDbContext: DbContext
{
  
}
