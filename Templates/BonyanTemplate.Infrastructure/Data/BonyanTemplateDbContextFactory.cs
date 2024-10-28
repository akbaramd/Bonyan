using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace BonyanTemplate.Infrastructure.Data;

public class BonyanTemplateDbContextFactory : IDesignTimeDbContextFactory<BonyanTemplateBookDbContext>
{
    public BonyanTemplateBookDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BonyanTemplateBookDbContext>();

        optionsBuilder.UseSqlite($"Data Source=../BonyanTemplate.Api/BonyanTemplate.db");

        return new BonyanTemplateBookDbContext(optionsBuilder.Options,new ServiceCollection().BuildServiceProvider());
    }
}