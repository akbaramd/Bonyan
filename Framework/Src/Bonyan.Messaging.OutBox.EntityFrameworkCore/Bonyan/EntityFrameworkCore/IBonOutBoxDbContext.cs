using Bonyan.Layer.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.EntityFrameworkCore;

public interface IBonOutBoxDbContext : IEfDbContext
{
    public  DbSet<BonOutboxMessage> OutboxMessages { get; set; }
}