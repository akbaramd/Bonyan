# Bonyan.UnitOfWork

## Table of Contents

- [Introduction](#introduction)
- [Installation](#installation)
- [The IBonUnitOfWork Interface](#the-ibonunitofwork-interface)
- [Usage](#usage)
  - [Enable via module](#enable-via-module)
  - [Attribute: `[BonUnitOfWork]`](#attribute-bonunitofwork)
  - [Convention: `IUnitOfWorkEnabled`](#convention-iunitofworkenabled)
  - [Manual: `IBonUnitOfWorkManager`](#manual-ibonunitofworkmanager)
  - [UoW items and events](#uow-items-and-events)
- [Tips and Tricks](#tips-and-tricks)
- [Summary](#summary)

---

## Introduction

`Bonyan.UnitOfWork` implements the **Unit of Work** pattern for .NET: a single scope in which multiple operations (e.g. repository calls) run and are committed or rolled back together. It integrates with the Bonyan modular application and (when used with `Bonyan.EntityFrameworkCore`) coordinates `DbContext` instances and database transactions.

**Benefits:**

- **Atomicity**: Either all changes in the unit of work are committed, or none are.
- **Automatic scoping**: Methods can run inside a UoW via attributes or interfaces, without manual `Begin`/`Complete` in every service.
- **Ambient UoW**: When a UoW is already active, nested calls reuse it (or create a new one with `requiresNew`).

---

## Installation

### 1. Add the package

```bash
dotnet add package Bonyan.UnitOfWork
```

### 2. Enable the module

Add `BonUnitOfWorkModule` to your application so that UoW services and the interceptor are registered:

```csharp
[DependsOn(
    typeof(BonUnitOfWorkModule)
    // ... other modules, e.g. Bonyan.EntityFrameworkCore
)]
public class MyApplicationModule : BonModule
{
}
```

For applications that use Entity Framework Core and need a UoW-scoped `DbContext`, also add the EF module (it depends on `BonUnitOfWorkModule` and registers the UoW-based DbContext provider):

```bash
dotnet add package Bonyan.EntityFrameworkCore
```

Then your DbContext will be created and shared within the current unit of work.

---

## The IBonUnitOfWork Interface

`IBonUnitOfWork` is the main abstraction for a unit of work. You usually do not implement it yourself; you obtain it from `IBonUnitOfWorkManager` or use it via the interceptor. Key members:

| Member | Description |
|--------|-------------|
| `Id` | Unique identifier of this UoW. |
| `Options` | Current options (transactional, isolation level, timeout). |
| `Outer` | Parent UoW when this one is nested. |
| `Items` | Dictionary to store arbitrary data for the lifetime of the UoW. |
| `ServiceProvider` | Scoped service provider for this UoW (e.g. to resolve DbContext). |
| `SaveChangesAsync()` | Flushes pending changes (e.g. `DbContext.SaveChangesAsync`) without committing the UoW. |
| `CompleteAsync()` | Saves changes, publishes events, commits transactions, and marks the UoW as completed. |
| `RollbackAsync()` | Rolls back the UoW. |
| `Failed` / `Disposed` | Events raised on failure or disposal. |
| `AddOrReplaceLocalEvent` / `AddOrReplaceDistributedEvent` | Register domain/integration events to be published when the UoW completes. |

It also extends `IDatabaseApiContainer` and `ITransactionApiContainer`, which are used by the EF integration to attach `DbContext` and transactions to the UoW.

---

## Usage

### Enable via module

After adding the package and depending on `BonUnitOfWorkModule`, the framework:

- Registers `IBonUnitOfWork`, `IBonUnitOfWorkManager`, and the ambient UoW accessor.
- Registers the **Unit of Work interceptor** for types/methods that are considered “UoW” (see below).

No extra configuration is required for the default behavior.

---

### Attribute: `[BonUnitOfWork]`

You can mark a **class** or a **method** with `[BonUnitOfWork]` so that calls to that method are wrapped in a unit of work.

**Class-level** (all public methods run inside a UoW):

```csharp
using Bonyan.UnitOfWork;

[BonUnitOfWork]
public class OrderAppService : IOrderAppService
{
    public async Task CreateOrderAsync(CreateOrderDto dto)
    {
        // Runs inside a UoW; changes committed on success, rolled back on exception.
    }
}
```

**Method-level** (only this method gets a UoW):

```csharp
[BonUnitOfWork]
public async Task PlaceOrderAsync(PlaceOrderInput input) { }

[BonUnitOfWork(IsTransactional = true, IsolationLevel = IsolationLevel.ReadCommitted)]
public async Task TransferAsync(TransferInput input) { }

[BonUnitOfWork(IsDisabled = true)]
public async Task GetReportAsync() { }  // No UoW; read-only or external scope.
```

**Options:**

- `IsTransactional`: use a database transaction (default can come from `BonUnitOfWorkDefaultOptions` or convention).
- `IsolationLevel`: transaction isolation level.
- `Timeout`: timeout in milliseconds.
- `IsDisabled = true`: do not start a UoW for this method (if there is already an ambient UoW, it is still used).

If a UoW is already active (e.g. from an outer service or middleware), the attribute does **not** start a new one; it reuses the ambient UoW.

---

### Convention: `IUnitOfWorkEnabled`

Implementing `IUnitOfWorkEnabled` makes **every public method** of the class run inside a unit of work (same as applying `[BonUnitOfWork]` at class level). No attribute is required.

```csharp
using Bonyan.UnitOfWork;

public class TestJob : IJob, IBonUnitOfWorkEnabled
{
    private readonly IBooksRepository _booksRepository;
    private readonly IAuthorsRepository _authorsRepository;

    public TestJob(IBooksRepository booksRepository, IAuthorsRepository authorsRepository)
    {
        _booksRepository = booksRepository;
        _authorsRepository = authorsRepository;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var author = await _authorsRepository.FindOneAsync(x => x.Title == "akbar ahmadi");
        if (author == null)
        {
            author = new Author { Title = "akbar ahmadi" };
            await _authorsRepository.AddAsync(author);
        }

        var book = new Book { Title = "DDD Books", Author = author };
        await _booksRepository.AddAsync(book);
        // All committed when the method completes successfully; rolled back on exception.
    }
}
```

This is convenient for application services, jobs, or handlers where the whole operation should be one unit of work.

---

### Manual: `IBonUnitOfWorkManager`

When you need explicit control (e.g. in a console app, middleware, or a method that should not be intercepted), use `IBonUnitOfWorkManager`:

```csharp
public class MyService
{
    private readonly IBonUnitOfWorkManager _uowManager;

    public MyService(IBonUnitOfWorkManager uowManager) => _uowManager = uowManager;

    public async Task DoWorkAsync()
    {
        using (var uow = _uowManager.Begin(new BonUnitOfWorkOptions { IsTransactional = true }))
        {
            // Use repositories / DbContext here; they will use the current UoW.
            await uow.CompleteAsync();
        }
    }
}
```

**Extension overloads:**

```csharp
// Simpler Begin with optional parameters
var uow = _uowManager.Begin(isTransactional: true, isolationLevel: IsolationLevel.ReadCommitted);

// Force a new UoW even if one is already active
var uow = _uowManager.Begin(options, requiresNew: true);
```

**Reserved UoW (e.g. for middleware):**

- `Reserve(name)`: create a UoW and reserve it under a name (e.g. `BonUnitOfWork.UnitOfWorkReservationName`).
- `TryBeginReserved(name, options)` / `BeginReserved(name, options)`: later, start the reserved UoW with the given options.

This allows a middleware to create a UoW at the start of a request and the interceptor to “join” it with `TryBeginReserved` instead of creating a second UoW.

---

### UoW items and events

**Items:** Use `Items` to attach data to the current UoW (e.g. request-scoped cache, correlation id):

```csharp
// Extensions from UnitOfWorkExtensions
currentUow.AddItem("CorrelationId", correlationId);
var id = currentUow.GetItemOrDefault<string>("CorrelationId");
var value = currentUow.GetOrAddItem("Cache", key => new MyCache());
currentUow.RemoveItem("Cache");
```

**Events:** Register events to be published when the UoW completes (after save and commit):

```csharp
currentUow.AddOrReplaceLocalEvent(new UnitOfWorkEventRecord(...));
currentUow.AddOrReplaceDistributedEvent(new UnitOfWorkEventRecord(...));
```

**Completion callback:**

```csharp
currentUow.OnCompleted(async () => await _notificationService.SendAsync(...));
```

---

## Tips and Tricks

1. **Read-only / no transaction**  
   For query-only methods, avoid transactions to reduce locking and improve performance:
   - Use `[BonUnitOfWork(IsTransactional = false)]`, or  
   - Rely on default behavior: methods whose name starts with `Get` (case-insensitive) are often treated as non-transactional when using the default transaction behaviour provider.

2. **Nested calls**  
   If `OrderAppService.CreateOrderAsync` calls `PaymentAppService.ChargeAsync`, and both are UoW methods, only one UoW is created (the inner call reuses the outer one). Use `requiresNew: true` only when you really need an independent scope.

3. **Dispose and exceptions**  
   Always use `using` when calling `Begin` manually. If you do not call `CompleteAsync()` (e.g. because of an exception), the UoW is still disposed and transactions are rolled back; the `Failed` event is raised.

4. **DbContext and UoW**  
   With `Bonyan.EntityFrameworkCore`, `DbContext` is resolved from the **current** UoW’s `ServiceProvider`. So:
   - Start the UoW (via attribute, `IUnitOfWorkEnabled`, or `Begin`) before using repositories/DbContext.
   - Do not resolve a scoped DbContext from the root container and expect it to participate in the UoW; use the same scope/UoW as the one that started the operation.

5. **Disable transactions globally**  
   To disable transactions for all UoWs (e.g. for testing or a provider that does not support them):
   ```csharp
   services.AddAlwaysDisableUnitOfWorkTransaction();
   ```

6. **Access current UoW**  
   Inject `IBonUnitOfWorkAccessor` (or `IBonAmbientBonUnitOfWork`) to get the current unit of work when needed:
   ```csharp
   var current = _uowAccessor.UnitOfWork;
   var currentOrNull = _ambientUow.GetCurrentByChecking();  // Skips reserved/disposed/completed.
   ```

7. **Child UoW**  
   When you call `Begin()` and a UoW is already current, the manager returns a **child** handle (same UoW, no new scope). Calling `CompleteAsync()` on that handle is a no-op; the real UoW is completed when the outer scope completes. This keeps nesting simple and avoids duplicate commits.

---

## Summary

- **Install**: `dotnet add package Bonyan.UnitOfWork` and add `BonUnitOfWorkModule` to your application (and `Bonyan.EntityFrameworkCore` if you use EF).
- **Use** `IBonUnitOfWork` via the interceptor: apply `[BonUnitOfWork]` on a class or method, or implement `IBonUnitOfWorkEnabled` for class-wide UoW.
- **Manual control**: inject `IBonUnitOfWorkManager`, call `Begin(...)`, use `using` and `CompleteAsync()`.
- **Leverage** `Items`, `OnCompleted`, and event records when you need request-scoped data or side effects after a successful commit.

By using `Bonyan.UnitOfWork` and `IBonUnitOfWork` as above, you get consistent, transactional behavior with minimal boilerplate.
