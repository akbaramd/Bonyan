# Application Module Guide

The `BonyanLayerApplicationModule` is a crucial part of the **Bonyan** architecture, designed to facilitate the application layer services by providing useful utilities and base classes. To use this module, you need to add `BonyanLayerApplicationModule` to your module.

```bash
dotnet add package Bonyan.Layer.Application
```
## Adding the Application Module

To make use of this module, add `BonyanLayerApplicationModule` to your target module's dependencies:

```csharp
public class YourTargetModule : Module
{
    public YourTargetModule()
    {
        DependOn<BonyanLayerApplicationModule>();
    }
}
```

This module also depends on `Bonyan.AutoMapperModule`, which means that when you add `BonyanLayerApplicationModule`, AutoMapper functionality will also be available, enabling easy object mapping and transformations.

## Application Service Base Classes

The **BonyanLayerApplicationModule** introduces base classes that simplify creating application-level services. The most important base classes are `IApplicationService` and `ApplicationService`.

### `IApplicationService` and `ApplicationService`

These classes form the foundation for creating services in the application layer.

- **`IApplicationService`** is an interface that defines the contract for application services.
- **`ApplicationService`** is an abstract class that implements `IApplicationService` and provides several commonly used properties to facilitate the development of application services.

Here's an example of `ApplicationService`:

```csharp
public class ApplicationService : LayServiceProviderConfigurator, IApplicationService
{
    public ICurrentUser CurrentUser => LazyServiceProvider.LazyGetRequiredService<ICurrentUser>();
    public ICurrentTenant CurrentTenant => LazyServiceProvider.LazyGetRequiredService<ICurrentTenant>();
    public IMapper Mapper => LazyServiceProvider.LazyGetRequiredService<IMapper>();
    protected IUnitOfWorkManager UnitOfWorkManager => LazyServiceProvider.LazyGetRequiredService<IUnitOfWorkManager>();
    protected IUnitOfWork? CurrentUnitOfWork => UnitOfWorkManager?.Current;
}
```

### Properties Available in `ApplicationService`

1. **`CurrentUser`**: Provides access to information about the current authenticated user. This is helpful for determining user-specific data or permissions.
2. **`CurrentTenant`**: Access to information about the current tenant, which is useful for multi-tenant applications to isolate data and configurations.
3. **`Mapper`**: Utilizes **AutoMapper** to perform object mappings between different models. This is helpful for transforming domain objects to DTOs or vice versa.
4. **`UnitOfWorkManager`**: Manages unit of work instances. This is crucial for managing transactional data changes across multiple repositories.
5. **`CurrentUnitOfWork`**: Represents the current unit of work if one is active. This helps ensure transactional consistency.

### Creating Application Services

You can use `ApplicationService` as a base class to create services that operate in the application layer. Here's a simple example:

```csharp
public class ProductAppService : ApplicationService
{
    private IProductRepository _productRepository => LazyServiceProvider.LazyGetRequiredService<IProductRepository>();

    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return Mapper.Map<ProductDto>(product);
    }
}
```

### Explanation

- **`ProductAppService`**: Inherits from `ApplicationService` to gain access to all the helpful properties like `Mapper`, `CurrentUser`, etc.
- **`IProductRepository`**: Instead of being injected through the constructor, `IProductRepository` is accessed via the `LazyServiceProvider`. This allows for **property injection** without requiring constructor parameters.
- **`Mapper.Map<ProductDto>(product)`**: Uses AutoMapper to map a `Product` entity to a `ProductDto`, simplifying object transformation logic.

This approach allows developers to easily access dependencies without cluttering constructors, providing a cleaner and more modular design.

## Built-in DTO Classes for Easy Conversion

The Bonyan application layer also provides some pre-prepared DTO classes that make it easier to convert entities to DTOs for common use cases. These include:

1. **`EntityDto`**: A base DTO class for regular entities.
2. **`AggregateRootDto`**: A DTO specifically for aggregate root entities.
3. **`FullAuditableAggregateRootDto`**: A DTO designed for fully auditable aggregate root entities.
4. **`PaginateDto`**: Helps with pagination needs.

    ```csharp
    public class PaginateDto
    {
        public int Take { get; set; }
        public int Skip { get; set; }
    }
    ```

5. **`FilterAndPaginateDto`**: Extends `PaginateDto` to add filtering capability.

    ```csharp
    public class FilterAndPaginateDto : PaginateDto
    {
        public string? Search { get; set; }
    }
    ```

These DTO classes provide a standard way to represent entities in a format that is easily consumed by clients or user interfaces.

### Example: Creating a Service with Entity to DTO Mapping

Here’s an example of how you can create an application service that gets entities from the repository and maps them to DTOs using the available tools in the Bonyan layer:

```csharp
public class OrderAppService : ApplicationService
{
    private IOrderRepository _orderRepository => LazyServiceProvider.LazyGetRequiredService<IOrderRepository>();

    public async Task<AggregateRootDto<OrderDto>> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        return Mapper.Map<AggregateRootDto<OrderDto>>(order);
    }

    public async Task<PaginateDto<OrderDto>> GetOrdersWithPaginationAsync(FilterAndPaginateDto input)
    {
        var orders = await _orderRepository.GetPaginatedListAsync(input.Skip, input.Take, input.Search);
        return new PaginateDto<OrderDto>
        {
            Take = input.Take,
            Skip = input.Skip,
            Items = Mapper.Map<List<OrderDto>>(orders)
        };
    }
}
```

### Explanation

- **`OrderAppService`**: Uses `ApplicationService` as its base class to have access to all useful properties and services.
- **`IOrderRepository`**: Accessed through the `LazyServiceProvider` for cleaner property injection.
- **`GetOrderByIdAsync`**: Retrieves an `Order` entity from the repository and maps it to `AggregateRootDto<OrderDto>` using the `Mapper`.
- **`GetOrdersWithPaginationAsync`**: Uses `FilterAndPaginateDto` to manage pagination and filtering, retrieves the paginated list from the repository, and maps it to a `PaginateDto` to be returned to the client.

## Built-in Dependencies and Features

The `BonyanLayerApplicationModule` comes with some built-in dependencies that facilitate the creation of services at the application level:

1. **AutoMapper Integration**: The `BonyanLayerApplicationModule` is dependent on `Bonyan.AutoMapperModule`, making AutoMapper automatically available for use in your application services. This allows for seamless mapping between different models, such as DTOs and domain objects.
2. **Dependency Injection**: The `ApplicationService` class uses Autofac internally to provide its injected properties, such as `CurrentUser`, `CurrentTenant`, and `UnitOfWorkManager`. This integration ensures that all required services are available when needed, without cluttering the constructors of application services.
3. **Unit of Work Management**: The `UnitOfWorkManager` and `CurrentUnitOfWork` properties help manage transactions across multiple operations, which is particularly useful when your service is interacting with multiple repositories and you need to ensure transactional consistency.

## Summary

The `BonyanLayerApplicationModule` makes it easy to create application-level services by providing a powerful base class (`ApplicationService`) that offers built-in access to many commonly used services, such as `CurrentUser`, `Mapper`, and `UnitOfWorkManager`. By using this module, you can focus on the business logic of your application services while relying on the infrastructure provided by Bonyan to handle the complexity of dependency injection and object mapping.

Additionally, using `LazyServiceProvider` allows you to access dependencies without requiring them to be injected via constructors, simplifying service creation and promoting cleaner, modular code.

The pre-prepared DTO classes, such as `EntityDto`, `AggregateRootDto`, `FullAuditableAggregateRootDto`, and `PaginateDto`, make it easier to convert entities to DTOs, streamlining the development process and ensuring a consistent approach to data transfer.

