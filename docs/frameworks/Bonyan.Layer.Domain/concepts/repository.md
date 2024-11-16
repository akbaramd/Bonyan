# Repository Abstractions in Bonyan.Layer.Domain

This guide explains the repository abstractions available in **Bonyan.Layer.Domain** and how you can leverage these abstractions to create efficient and reusable repository patterns. The repositories integrate seamlessly with the specification pattern, allowing you to define query criteria in a clean and reusable manner.

## Overview
The repository pattern is a key element in domain-driven design (DDD). It provides a way to encapsulate data access logic, allowing domain entities to remain unaware of the specifics of data storage. Repositories serve as a bridge between the domain and data mapping layers by mediating queries to the underlying data storage.

In **Bonyan.Layer.Domain**, the repository abstractions are designed to support both read-only and full CRUD operations. Additionally, these repositories integrate smoothly with specifications, allowing developers to keep query logic separate and reusable.

## Repository Interfaces
The following repository abstractions are provided by **Bonyan.Layer.Domain** to facilitate different use cases.

### IBonReadOnlyRepository<TEntity>
The `IBonReadOnlyRepository<TEntity>` interface provides read-only operations for entities without requiring a key type. It includes methods for querying, finding, and counting entities:

- **Queryable**: Gets the queryable interface for the entity.
- **FindAsync**: Finds entities that match a predicate.
- **FindOneAsync**: Finds a single entity based on a predicate.
- **GetOneAsync**: Gets a single entity based on a predicate, throwing an exception if no entity is found.
- **CountAsync**: Counts the entities that match a predicate.
- **ExistsAsync**: Checks if an entity exists based on a predicate.
- **PaginatedAsync**: Retrieves paginated results using either a predicate or a specification.

### IBonReadOnlyRepository<TEntity, TKey>
The `IBonReadOnlyRepository<TEntity, TKey>` interface extends `IBonReadOnlyRepository<TEntity>` and adds support for finding entities by their unique key:

- **FindByIdAsync**: Finds an entity by its key.
- **GetByIdAsync**: Gets an entity by its key, throwing an exception if not found.

### IBonRepository<TEntity>
The `IBonRepository<TEntity>` interface extends the read-only repository to provide full CRUD operations:

- **AddAsync**: Adds a new entity.
- **UpdateAsync**: Updates an existing entity.
- **DeleteAsync**: Deletes an entity.
- **AddRangeAsync / UpdateRangeAsync / DeleteRangeAsync**: Bulk operations for adding, updating, and deleting multiple entities.

### IBonRepository<TEntity, TKey>
The `IBonRepository<TEntity, TKey>` interface provides CRUD operations for entities with a specific key type. It extends `IBonReadOnlyRepository<TEntity, TKey>` and adds:

- **DeleteByIdAsync**: Deletes an entity by its key.

### BonPaginatedResult<T>
The `BonPaginatedResult<T>` class provides a structure for handling paginated results, including information about the total count, page size, and current page:

- **Results**: The list of results for the current page.
- **Skip / Take**: Pagination details indicating the range of items.
- **TotalCount**: The total number of items.
- **TotalPages**: The total number of pages available.

## Using Repository Abstractions
These repository abstractions are designed to be used in combination with domain entities and specifications to create a clean, maintainable data access layer. Here’s an example of using the read-only repository with a specification:

### Example: Using IBonReadOnlyRepository with a Specification
Suppose you have a `Customer` entity, and you need to filter customers by city. You can define a specification and use it with the repository as follows:

```csharp
using Bonyan.Layer.Domain.Repository.Abstractions;
using Bonyan.Layer.Domain.Specifications;

public class CustomerRepository
{
    private readonly IBonReadOnlyRepository<Customer> _repository;

    public CustomerRepository(IBonReadOnlyRepository<Customer> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Customer>> GetCustomersByCityAsync(string city)
    {
        var specification = new CustomerByCitySpecification(city);
        return await _repository.FindAsync(specification);
    }
}
```

In this example, the `CustomerRepository` uses `IBonReadOnlyRepository<Customer>` to access data. The `CustomerByCitySpecification` is passed to the repository to apply the necessary filtering logic, keeping the query criteria separate from the repository.

### Reference to Specifications
To learn more about creating specifications, refer to the [Specifications in Bonyan.Layer.Domain](#specifications-in-bonyan-layerdomain) document, which provides detailed information on defining and using specifications effectively.

## Creating Custom Repository Contracts

To implement your own repository contracts using the abstractions provided by **Bonyan.Layer.Domain**, follow these steps to create a consistent, maintainable repository layer that fits your specific needs.

### Step 1: Create a Custom Repository Interface
Define your own repository interface by extending the available abstractions. For example, if you need a custom repository for a `Customer` entity, you can create an interface that extends `IBonRepository<Customer>` or `IBonReadOnlyRepository<Customer>` as needed.

```csharp
using Bonyan.Layer.Domain.Repository.Abstractions;

public interface ICustomerRepository : IBonRepository<Customer>
{
    Task<IEnumerable<Customer>> GetCustomersByCityAsync(string city);
}
```

In this example, `ICustomerRepository` extends `IBonRepository<Customer>`, which includes all CRUD operations, and adds a custom method for getting customers by city.

### Step 2: Implement the Custom Repository
Create a concrete implementation of your custom repository interface. Here, you can use the `BonSpecificationContext` to manage the state of your queries, and you can also integrate specifications to encapsulate filtering logic.

```csharp
using Bonyan.Layer.Domain.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

public class CustomerRepository : ICustomerRepository
{
    private readonly DbContext _context;

    public CustomerRepository(DbContext context)
    {
        _context = context;
    }

    public IQueryable<Customer> Queryable => _context.Set<Customer>();

    public async Task<IEnumerable<Customer>> GetCustomersByCityAsync(string city)
    {
        return await _context.Set<Customer>()
            .Where(c => c.Address.City == city)
            .ToListAsync();
    }

    public async Task<Customer> AddAsync(Customer entity, bool autoSave = false)
    {
        await _context.Set<Customer>().AddAsync(entity);
        if (autoSave) await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Customer entity, bool autoSave = false)
    {
        _context.Set<Customer>().Update(entity);
        if (autoSave) await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Customer entity, bool autoSave = false)
    {
        _context.Set<Customer>().Remove(entity);
        if (autoSave) await _context.SaveChangesAsync();
    }

    // Other methods from IBonRepository...
}
```

### Step 3: Using Custom Repositories in Your Application
Once you have your custom repository implemented, you can use it throughout your application to perform data access operations while keeping your domain logic clean and separated.

```csharp
public class CustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<Customer>> GetCustomersInCityAsync(string city)
    {
        return await _customerRepository.GetCustomersByCityAsync(city);
    }
}
```

In this example, `CustomerService` depends on `ICustomerRepository`, which encapsulates all data access logic, making the service layer focus purely on business logic.

### Reference to Specifications
When implementing complex queries, consider using specifications to encapsulate filtering logic. For more details on creating and using specifications, see the [Specifications in Bonyan.Layer.Domain](/frameworks/Bonyan.Layer.Domain/concepts/specification.md) document.

## Summary
- **Repository Abstractions**: The repository interfaces (`IBonReadOnlyRepository`, `IBonRepository`) provide read and write operations for entities while hiding the details of data access.
- **Key Types**: Interfaces are available for both keyless entities and entities with unique keys (`TKey`).
- **Integration with Specifications**: Repositories work well with specifications, allowing you to encapsulate complex query logic in a reusable way.
- **Pagination Support**: Use `BonPaginatedResult` for handling paginated queries effectively.

By leveraging the repository and specification patterns in **Bonyan.Layer.Domain**, you can create a clean, maintainable, and flexible data access layer that adheres to DDD principles.

