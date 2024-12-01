using Bonyan.EntityFrameworkCore;
using Bonyan.Layer.Domain;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Demo.Data;

public class AppDBContext : BonDbContext<AppDBContext>,IBonOutBoxDbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
    }

    public DbSet<BonOutboxMessage> OutboxMessages { get; set; }
}