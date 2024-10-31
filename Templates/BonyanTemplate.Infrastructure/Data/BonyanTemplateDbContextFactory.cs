using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BonyanTemplate.Infrastructure.Data;

public class BonyanTemplateDbContextFactory : IDesignTimeDbContextFactory<BonyanTemplateBookManagementDbContext>
{
    public BonyanTemplateBookManagementDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BonyanTemplateBookManagementDbContext>();

        optionsBuilder.UseSqlite($"Data Source=../BonyanTemplate.Api/BonyanTemplate.db");

        var xtx =  new BonyanTemplateBookManagementDbContext(optionsBuilder.Options);
        return xtx;
    }
}