using Bonyan.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bonyan.UnitOfWork;

/// <summary>
/// Interceptor that wraps UoW methods in a unit of work. Uses the current scope's manager (no extra scope).
/// Reserved UoW branch: only proceeds; outer middleware is responsible for CompleteAsync.
/// </summary>
public class BonUnitOfWorkInterceptor : BonInterceptor
{
    private readonly IBonUnitOfWorkManager _unitOfWorkManager;
    private readonly IOptions<BonUnitOfWorkDefaultOptions> _defaultOptions;
    private readonly IUnitOfWorkTransactionBehaviourProvider _transactionBehaviourProvider;

    public BonUnitOfWorkInterceptor(
        IBonUnitOfWorkManager unitOfWorkManager,
        IOptions<BonUnitOfWorkDefaultOptions> defaultOptions,
        IUnitOfWorkTransactionBehaviourProvider transactionBehaviourProvider)
    {
        _unitOfWorkManager = unitOfWorkManager ?? throw new ArgumentNullException(nameof(unitOfWorkManager));
        _defaultOptions = defaultOptions ?? throw new ArgumentNullException(nameof(defaultOptions));
        _transactionBehaviourProvider = transactionBehaviourProvider ?? throw new ArgumentNullException(nameof(transactionBehaviourProvider));
    }

    public override async Task InterceptAsync(IBonMethodInvocation invocation)
    {
        if (!BonUnitOfWorkHelper.IsUnitOfWorkMethod(invocation.Method, out var unitOfWorkAttribute))
        {
            await invocation.ProceedAsync();
            return;
        }

        var options = CreateOptions(invocation, unitOfWorkAttribute);

        // Outer layer (e.g. middleware) already started a reserved UoW; just run inside it.
        if (_unitOfWorkManager.TryBeginReserved(BonUnitOfWork.UnitOfWorkReservationName, options))
        {
            await invocation.ProceedAsync();
            return;
        }

        using (var uow = _unitOfWorkManager.Begin(options))
        {
            await invocation.ProceedAsync();
            await uow.CompleteAsync();
        }
    }

    private BonUnitOfWorkOptions CreateOptions(IBonMethodInvocation invocation, BonUnitOfWorkAttribute? unitOfWorkAttribute)
    {
        var options = new BonUnitOfWorkOptions();
        unitOfWorkAttribute?.SetOptions(options);

        if (unitOfWorkAttribute?.IsTransactional == null)
        {
            options.IsTransactional = _defaultOptions.Value.CalculateIsTransactional(
                autoValue: _transactionBehaviourProvider.IsTransactional
                    ?? !invocation.Method.Name.StartsWith("Get", StringComparison.InvariantCultureIgnoreCase));
        }

        return options;
    }
}
