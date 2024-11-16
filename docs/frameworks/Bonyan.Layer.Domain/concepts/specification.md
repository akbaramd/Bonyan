# Specifications in Bonyan.Layer.Domain

This guide explains the concept of specifications in **Bonyan.Layer.Domain** and how to use them effectively, especially in the context of the repository pattern.

## Overview
The specification pattern is a design pattern used in domain-driven design to encapsulate query logic in a reusable, composable, and testable way. Specifications help you define the criteria for filtering and managing entities without hard-coding the query logic into your repositories.

In **Bonyan.Layer.Domain**, specifications are implemented using the `BonSpecification<T>` class and related classes to define the criteria that entities must meet.

## BonSpecification Base Class
The **BonSpecification** base class provides a foundation for defining specifications for domain entities. It uses the `Handle` method to apply specific rules or filters to a given context.

Here's a basic definition of the `BonSpecification` class:

```csharp
using Bonyan.Layer.Domain.Specification.Abstractions;

namespace Bonyan.Layer.Domain.Specifications;

public abstract class BonSpecification<T> : IBonSpecification<T> where T : class
{
  // The Handle method will be implemented by derived classes
  public abstract void Handle(IBonSpecificationContext<T> context);
}
```

To create a specification, you inherit from `BonSpecification<T>` and implement the `Handle` method to define the query logic that should be applied to the context.

## BonSpecificationContext Class
The `BonSpecificationContext<T>` class is responsible for managing the state of a query and applying various criteria or operations to it. It provides a flexible way to modify the query based on specification logic.

The `BonSpecificationContext<T>` class provides several methods to help manage query state and apply various criteria effectively:
- **ApplyOrderBy**: Orders the query by a specified key.
- **ApplyOrderByDescending**: Orders the query in descending order by a specified key.
- **AddInclude**: Includes related entities in the query.
- **ApplyWhereIn**: Filters the query based on a list of values (similar to SQL IN clause).
- **ApplyDistinct**: Ensures only distinct results are returned.
- **ApplyGroupBy**: Groups the query results by a specified key.
- **ApplyMax**: Retrieves the maximum value for a specified property.
- **ApplyMin**: Retrieves the minimum value for a specified property.

## Defining a Specification
Below is an example of defining a specification to filter customers by city:

```csharp
using Bonyan.Layer.Domain.Specifications;
using Bonyan.Layer.Domain.Specification.Abstractions;

public class CustomerByCitySpecification : BonSpecification<Customer>
{
    private readonly string _city;

    public CustomerByCitySpecification(string city)
    {
        _city = city;
    }

    public override void Handle(IBonSpecificationContext<Customer> context)
    {
        context.AddCriteria(customer => customer.Address.City == _city);
    }
}
```

In this example, `CustomerByCitySpecification` encapsulates the filtering logic for customers based on the city.

## Using Specifications with Repository Pattern
The specification pattern is often used in combination with the repository pattern to separate the domain logic from the data access logic. However, repositories should generally be accessed through services to maintain a clean separation between business logic and data access.

In a service class, you can use a repository that accepts a specification to filter entities like this:

```csharp
public class CustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<Customer>> GetCustomersByCityAsync(string city)
    {
        var specification = new CustomerByCitySpecification(city);
        return await _customerRepository.FindAsync(specification);
    }
}
```

In this example, the `CustomerService` class depends on `ICustomerRepository` to access customer data. The `CustomerByCitySpecification` encapsulates the filtering logic, and the repository applies this logic to return the matching customers. This setup ensures that business logic remains in the service layer, keeping the data access layer (repository) clean and focused on data retrieval.
}
}
```

In this example, the repository accepts a specification and applies it to the query using `BonSpecificationContext`. This approach allows for reusable, composable query logic that is easy to test and extend.

## Example: Pagination with Specifications
You can also use specifications to implement pagination in your repository. Here’s an example that demonstrates how to use `take` and `skip` parameters with a specification to return paginated results:

```csharp
public class CustomerRepository
{
    private readonly IBonReadOnlyRepository<Customer> _repository;

    public CustomerRepository(IBonReadOnlyRepository<Customer> repository)
    {
        _repository = repository;
    }

    public async Task<BonPaginatedResult<Customer>> GetCustomersWithPaginationAsync(string city, int take, int skip)
    {
        var specification = new CustomerByCitySpecification(city);
        return await _repository.PaginatedAsync(specification, take, skip);
    }
}
```

In this example, the `GetCustomersWithPaginationAsync` method demonstrates how to use pagination (`take` and `skip` parameters) with a specification to return paginated results.

## Summary
- **BonSpecification**: Base class for defining specifications that encapsulate query logic.
- **BonSpecificationContext**: Manages query state and applies various filters and criteria.
- **Repository Pattern Integration**: Specifications can be used in repositories to apply filtering, sorting, and other criteria, keeping data access logic clean and reusable.

Specifications in **Bonyan.Layer.Domain** help create a flexible, reusable way to define filtering and ordering logic, especially when used alongside the repository pattern. This approach promotes cleaner, more maintainable code and ensures the separation of concerns between domain logic and data access.

