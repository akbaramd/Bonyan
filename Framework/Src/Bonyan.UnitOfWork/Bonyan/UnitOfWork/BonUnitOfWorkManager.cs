using Bonyan.Core;
using Bonyan.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.UnitOfWork;

public class BonUnitOfWorkManager : IBonUnitOfWorkManager
{
    [Obsolete("This will be removed in next versions.")]
    public static AsyncLocal<bool> DisableObsoleteDbContextCreationWarning { get; } = new();

    public IBonUnitOfWork? Current => _bonAmbientBonUnitOfWork.GetCurrentByChecking();

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IBonAmbientBonUnitOfWork _bonAmbientBonUnitOfWork;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1,1);
    public BonUnitOfWorkManager(
        IBonAmbientBonUnitOfWork bonAmbientBonUnitOfWork,
        IServiceScopeFactory serviceScopeFactory)
    {
        _bonAmbientBonUnitOfWork = bonAmbientBonUnitOfWork;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public IBonUnitOfWork Begin(BonUnitOfWorkOptions options, bool requiresNew = false)
    {
        Check.NotNull(options, nameof(options));

         _semaphore.Wait();
        
        var currentUow = Current;
        if (currentUow != null && !requiresNew)
        {
            return new BonChildUnitOfWork(currentUow);
        }

        var unitOfWork = CreateNewUnitOfWork();
        unitOfWork.Initialize(options);

        return unitOfWork;
    }

    public IBonUnitOfWork Reserve(string reservationName, bool requiresNew = false)
    {
        Check.NotNull(reservationName, nameof(reservationName));

        if (!requiresNew &&
            _bonAmbientBonUnitOfWork.UnitOfWork != null &&
            _bonAmbientBonUnitOfWork.UnitOfWork.IsReservedFor(reservationName))
        {
            return new BonChildUnitOfWork(_bonAmbientBonUnitOfWork.UnitOfWork);
        }

        var unitOfWork = CreateNewUnitOfWork();
        unitOfWork.Reserve(reservationName);

        return unitOfWork;
    }

    public void BeginReserved(string reservationName, BonUnitOfWorkOptions options)
    {
        if (!TryBeginReserved(reservationName, options))
        {
            throw new BonException($"Could not find a reserved unit of work with reservation name: {reservationName}");
        }
    }

    public bool TryBeginReserved(string reservationName, BonUnitOfWorkOptions options)
    {
        Check.NotNull(reservationName, nameof(reservationName));

        var uow = _bonAmbientBonUnitOfWork.UnitOfWork;

        //Find reserved unit of work starting from current and going to outers
        while (uow != null && !uow.IsReservedFor(reservationName))
        {
            uow = uow.Outer;
        }

        if (uow == null)
        {
            return false;
        }

        uow.Initialize(options);

        return true;
    }

    private IBonUnitOfWork CreateNewUnitOfWork()
    {
        var scope = _serviceScopeFactory.CreateScope();
        try
        {
            var outerUow = _bonAmbientBonUnitOfWork.UnitOfWork;

            var unitOfWork = scope.ServiceProvider.GetRequiredService<IBonUnitOfWork>();

            unitOfWork.SetOuter(outerUow);

            _bonAmbientBonUnitOfWork.SetUnitOfWork(unitOfWork);

            unitOfWork.Disposed += (sender, args) =>
            {
                _bonAmbientBonUnitOfWork.SetUnitOfWork(outerUow);
                scope.Dispose();
            };

            return unitOfWork;
        }
        catch
        {
            scope.Dispose();
            throw;
        }
    }
    
    
}
