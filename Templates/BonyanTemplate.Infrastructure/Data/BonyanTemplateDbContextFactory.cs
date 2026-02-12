using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BonyanTemplate.Infrastructure.Data;

public class BonyanTemplateDbContextFactory : IDesignTimeDbContextFactory<BonyanTemplateDbContext>
{
    public BonyanTemplateDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BonyanTemplateDbContext>();

        // Design-time and local development: use local SQL Server (LocalDB).
        // At runtime the app uses the same connection from the module.
        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=BonyanTemplate;Trusted_Connection=True;TrustServerCertificate=True;";
        optionsBuilder.UseSqlServer(connectionString);

        return new BonyanTemplateDbContext(optionsBuilder.Options);
    }
}