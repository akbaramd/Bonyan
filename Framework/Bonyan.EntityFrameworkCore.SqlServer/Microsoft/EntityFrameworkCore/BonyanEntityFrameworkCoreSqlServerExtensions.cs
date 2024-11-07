using Bonyan.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore;

public static class BonyanEntityFrameworkCoreSqlServerExtensions
{
    public static BonEntityFrameworkDbContextOptions UseSqlServer(this BonEntityFrameworkDbContextOptions options,
        string connectionStrings)
    {
        return options.Configure(c => { c.UseSqlServer(connectionStrings); });
    }
}