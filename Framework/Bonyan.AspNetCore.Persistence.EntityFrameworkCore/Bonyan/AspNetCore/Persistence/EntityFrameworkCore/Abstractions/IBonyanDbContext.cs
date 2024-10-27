using Microsoft.EntityFrameworkCore;

namespace Bonyan.AspNetCore.Persistence.EntityFrameworkCore.Abstractions;

public interface IBonyanDbContext<TDbContext> : IDisposable where TDbContext: DbContext
{
  
}
