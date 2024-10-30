using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BonyanTemplate.Infrastructure.Data;

public class BonyanTemplateDbContextFactory : IDesignTimeDbContextFactory<BonyanTemplateBookDbContext>
{
    public BonyanTemplateBookDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BonyanTemplateBookDbContext>();

        optionsBuilder.UseSqlite($"Data Source=../BonyanTemplate.Api/BonyanTemplate.db");

        var xtx =  new BonyanTemplateBookDbContext(optionsBuilder.Options);
        return xtx;
    }
}