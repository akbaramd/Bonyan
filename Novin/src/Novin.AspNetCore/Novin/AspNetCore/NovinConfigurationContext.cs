using Bonyan.Modularity;

namespace Novin.AspNetCore.Novin.AspNetCore;

public class NovinConfigurationContext
{
    public NovinConfigurationContext(BonConfigurationContext bonContext)
    {
        BonContext = bonContext;
    }

    public BonConfigurationContext BonContext { get; set; }
    
}

public class NovinApplicationContext
{
    public NovinApplicationContext(BonWebApplicationContext bonAppContext)
    {
        BonAppContext = bonAppContext;
    }

    public BonWebApplicationContext BonAppContext { get; set; }
    
}