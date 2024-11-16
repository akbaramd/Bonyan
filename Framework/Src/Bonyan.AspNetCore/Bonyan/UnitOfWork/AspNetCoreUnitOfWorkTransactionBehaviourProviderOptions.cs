namespace Bonyan.UnitOfWork;

public class AspNetCoreUnitOfWorkTransactionBehaviourProviderOptions
{
    public List<string> NonTransactionalUrls { get; }

    public AspNetCoreUnitOfWorkTransactionBehaviourProviderOptions()
    {
        NonTransactionalUrls = new List<string>
            {
                "/connect/"
            };
    }
}
