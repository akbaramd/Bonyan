using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BonyanTemplate.Infrastructure.Data;

public class BonyanTemplateDbContextFactory : IDesignTimeDbContextFactory<BonyanTemplateDbContext>
{
    public BonyanTemplateDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BonyanTemplateDbContext>();

        optionsBuilder.UseSqlite("Data Source=BonyanTemplate.db");

        return new BonyanTemplateDbContext(optionsBuilder.Options);
    }
}