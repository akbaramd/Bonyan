using Bonyan.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore;

public static class BonyanEntityFrameworkCoreSqlServerExtensions
{
    public static EntityFrameworkDbContextOptions UseSqlServer(this EntityFrameworkDbContextOptions options,
        string connectionStrings)
    {
        return options.Configure(c => { c.UseSqlServer(connectionStrings); });
    }
}