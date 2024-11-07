using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BonyanTemplate.Infrastructure.Data;

public class BonyanTemplateDbContextFactory : IDesignTimeDbContextFactory<BonTemplateBookManagementDbContext>
{
    public BonTemplateBookManagementDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BonTemplateBookManagementDbContext>();

        optionsBuilder.UseSqlite($"Data Source=../BonyanTemplate.Api/BonyanTemplate.db");

        var xtx =  new BonTemplateBookManagementDbContext(optionsBuilder.Options);
        return xtx;
    }
}