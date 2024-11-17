using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BonyanTemplate.Infrastructure.Data;

public class BonyanTemplateDbContextFactory : IDesignTimeDbContextFactory<TemplateBookManagementBonDbContext>
{
    public TemplateBookManagementBonDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TemplateBookManagementBonDbContext>();

        optionsBuilder.UseSqlite($"Data Source=../BonyanTemplate.WebApi/BonyanTemplate.db");

        var xtx =  new TemplateBookManagementBonDbContext(optionsBuilder.Options);
        return xtx;
    }
}