using Bonyan.EntityFrameworkCore;
using Bonyan.EntityFrameworkCore.Abstractions;

namespace Microsoft.EntityFrameworkCore;

public static class BonyanEntityFrameworkCoreSqlServerExtensions
{
    public static IBonDbContextRegistrationOptionBuilder UseSqlServer(this IBonDbContextRegistrationOptionBuilder options,
        string connectionStrings)
    {
        return options.Configure(c => { c.UseSqlServer(connectionStrings); });
    }
    
}