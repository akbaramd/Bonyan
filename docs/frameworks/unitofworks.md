# Bonyan.UnitOfWork

## Introduction

`Bonyan.UnitOfWork` is a powerful library designed to simplify the implementation of the Unit of Work pattern in .NET applications. By using the `IUnitOfWorkEnabled` interface, developers can ensure that repository and application service methods are automatically executed within a transactional unit of work. This guarantees that either all operations succeed and changes are committed, or none are, maintaining data integrity.
```bash
dotnet add package Bonyan.UnitOfWork
```
## Key Features
- **Automatic Unit of Work Handling**: All methods within classes that implement `IUnitOfWorkEnabled` are executed in a transactional unit of work.
- **Consistent Data State**: If an error occurs during method execution, no data will be committed to the database, ensuring consistent and reliable data state.
- **Simplified Code**: Developers no longer need to manually manage transactions for each operation.

## How It Works
Classes such as repositories and application services can inherit from the `IUnitOfWorkEnabled` interface, allowing them to automatically benefit from the unit of work functionality. All methods called within a class implementing `IUnitOfWorkEnabled` are executed within a unit of work scope. This means that:

- If no exceptions occur during the execution of a method, all changes are committed.
- If an exception is thrown, none of the changes are saved to the database, maintaining data consistency.

### Example
Here's a simplified example of how to use `Bonyan.UnitOfWork` in a job class:

```csharp
using Bonyan.UnitOfWork;
using BonyanTemplate.Domain.Repositories;
using BonyanTemplate.Domain.Entities;

namespace BonyanTemplate.Application.Jobs;

public class TestJob : IJob, IUnitOfWorkEnabled
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
            author = new Authors() { Title = "akbar ahmadi" };
            await _authorsRepository.AddAsync(author);
        }

        var book = new Books() { Title = "DDD Books", Status = BookStatus.Available, Author = author };
        await _booksRepository.AddAsync(book);

        book.Status = BookStatus.OutOfStock;
        await _booksRepository.UpdateAsync(book);
    }
}
```

In this example, the `TestJob` class implements `IUnitOfWorkEnabled`, which means the `ExecuteAsync` method is executed inside a transactional unit of work scope. If no exceptions occur during the execution, the changes are successfully committed. However, if any error occurs, none of the changes are saved, ensuring the database remains consistent.

## Configuration
To use the `Bonyan.UnitOfWork` library, you must add the `BonyanUnitOfWorkModule` to your application. This ensures that the unit of work is enabled and functional throughout your application's modules.

### Steps to Enable Unit of Work
1. **Add the Module**: Include the `BonyanUnitOfWorkModule` in your module (Domain/Application/Entity Framework).
2. **Automatic Configuration**: Once the module is added, the unit of work functionality is automatically enabled, and there is no need for extra configuration.

## Summary
- The `IUnitOfWorkEnabled` interface provides automatic unit of work functionality to classes, ensuring that operations are transactional.
- Repository operations (`AddAsync`, `UpdateAsync`, etc.) are only committed if no exceptions occur.
- To use this feature, simply add the `BonyanUnitOfWorkModule` to your application.

By using `Bonyan.UnitOfWork`, you can create reliable and maintainable .NET applications with minimal overhead and maximum consistency in managing your data.

