# Entity Framework Core Persistence Module Guide

The **Bonyan.AspNetCore.Persistence.EntityFrameworkCore** module integrates Entity Framework Core with the Bonyan Modular Application Framework to provide developers with powerful persistence capabilities. By using this module, developers can efficiently implement **Domain-Driven Design (DDD)** principles for data access and management, leveraging the full capabilities of **Entity Framework Core** in .NET Core applications. This guide aims to comprehensively explain how to set up and effectively use this module, ensuring that even the most complex domain models can be represented and managed with ease.

> **Note**: If you are unfamiliar with DDD, please refer to the [Domain-Driven Design Module Guide](ddd_domain_module_guide.md) for an overview of DDD concepts such as Entities, Aggregate Roots, and Value Objects.

## Table of Contents
- [Introduction](#introduction)
- [Installation](#installation)
- [Setting Up Entity Framework Core](#setting-up-entity-framework-core)
    - [DbContext Configuration](#dbcontext-configuration)
    - [Repository Implementation](#repository-implementation)
- [Usage Examples](#usage-examples)
    - [Defining a DbContext](#defining-a-dbcontext)
    - [Implementing a Repository](#implementing-a-repository)
- [SQL Provider Configuration](#sql-provider-configuration)
    - [SQLite Setup](#sqlite-setup)
    - [SQL Server Setup](#sql-server-setup)
- [Best Practices and Considerations](#best-practices-and-considerations)
- [Summary](#summary)

## Introduction

The **Bonyan.AspNetCore.Persistence.EntityFrameworkCore** module is built to provide a seamless integration with Entity Framework Core, enabling developers to implement persistence strategies following Domain-Driven Design principles. By extending the Bonyan framework, this module allows for creating **DbContexts**, **Repositories**, and handling complex data models efficiently. The module is particularly useful for managing **Aggregate Roots** and other entities that require persistence across different data stores, thereby ensuring consistency, reliability, and scalability within your software application.

This module also supports a variety of relational database providers, allowing developers to choose between **SQLite**, **SQL Server**, or any other database compatible with Entity Framework Core. By combining these powerful technologies, the module enables developers to focus on modeling their domain without worrying about the low-level details of database interaction and configuration.

## Installation

To install the **Bonyan.AspNetCore.Persistence.EntityFrameworkCore** module, run the following command in your terminal:

```bash
dotnet add package Bonyan.AspNetCore.Persistence.EntityFrameworkCore
```

This command will include the necessary libraries and tools for integrating Entity Framework Core into your Bonyan-based application. After installing the package, you will be ready to start setting up your database context and repositories to interact seamlessly with your domain models.

## Setting Up Entity Framework Core

Setting up Entity Framework Core with the Bonyan framework requires defining a **DbContext** that interacts with your domain models and implementing repositories to abstract away data access logic.

### DbContext Configuration

To use this module, your primary database context must inherit from `BonyanDbContext`. This will ensure that all domain-driven design components, such as entities and aggregate roots, are appropriately configured and integrated. The `BonyanDbContext` provides additional scaffolding and utility methods that make it easier to work with common DDD patterns, including conventions and automatic configuration of related entities.

```csharp
public class BonyanTemplateBookDbContext : BonyanDbContext<BonyanTemplateBookDbContext>
{
    public BonyanTemplateBookDbContext(DbContextOptions<BonyanTemplateBookDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Books>().ConfigureByConvention();
        modelBuilder.Entity<Authors>().ConfigureByConvention();
    }

    public DbSet<Books> Books { get; set; }
    public DbSet<Authors> Authors { get; set; }
}
```

In this example, `BonyanTemplateBookDbContext` inherits from `BonyanDbContext` to provide all the basic functionality needed for integrating Entity Framework with DDD elements such as entities and aggregate roots. The `ConfigureByConvention()` method simplifies setting up default conventions, ensuring consistency across the application.

The `OnModelCreating` method is used to define and configure the relationships between entities, which can include specifying primary keys, foreign keys, constraints, and any additional configurations that may be needed to properly reflect the domain model in the database.

### Repository Implementation

Repositories are a key component in managing data persistence in DDD. They abstract the data access logic, making it easier to perform CRUD operations on entities, while maintaining the principles of encapsulation and isolation from infrastructure-level details. This enables developers to focus more on the business logic rather than the intricacies of data access.

```csharp
public class EfBookRepository : EfCoreRepository<Books, BookId, BonyanTemplateBookDbContext>, IBooksRepository
{
    public EfBookRepository(BonyanTemplateBookDbContext bookDbContext, IServiceProvider serviceProvider) : base(bookDbContext, serviceProvider)
    {
    }
}
```

In this example, `EfBookRepository` is a repository class that manages `Books` entities by extending the `EfCoreRepository` base class. It handles typical operations like adding, removing, and querying `Books` from the database, while remaining decoupled from the infrastructure specifics. This abstraction is essential for ensuring that the domain model remains agnostic to the underlying persistence technology, allowing changes in the persistence layer without impacting the domain logic.

## Usage Examples

### Defining a DbContext

To define a `DbContext` in the Bonyan framework, you need to inherit from `BonyanDbContext`. This base class provides the scaffolding needed to integrate your domain entities with Entity Framework Core seamlessly. It also handles lifecycle events and hooks that you can override to add custom logic during various phases of model creation or context initialization.

```csharp
public class CustomDbContext : BonyanDbContext<CustomDbContext>
{
    public CustomDbContext(DbContextOptions<CustomDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<MyEntity>().ConfigureByConvention();
    }

    public DbSet<MyEntity> MyEntities { get; set; }
}
```

In this example, the `CustomDbContext` class manages a set of entities represented by `MyEntity`. The `ConfigureByConvention` method automatically applies default rules for entity properties, making it easier to enforce uniformity across entities.

### Implementing a Repository

Repositories provide an abstraction over the persistence logic, which helps keep your domain model focused on the business logic rather than data access concerns. This decouples the domain model from infrastructure concerns and provides a consistent interface for accessing data.

```csharp
public class EfCustomRepository : EfCoreRepository<MyEntity, MyEntityId, CustomDbContext>, IMyEntityRepository
{
    public EfCustomRepository(CustomDbContext dbContext, IServiceProvider serviceProvider) : base(dbContext, serviceProvider)
    {
    }
}
```

In this example, `EfCustomRepository` is an implementation of the repository pattern that manages the persistence operations for `MyEntity`. By inheriting from `EfCoreRepository`, the repository gains access to generic persistence operations, ensuring consistency in the application's data layer while minimizing redundant code. This pattern makes it easier to replace or modify persistence logic without affecting other parts of the system.

## SQL Provider Configuration

The **Bonyan.AspNetCore.Persistence.EntityFrameworkCore** module supports various SQL providers. Below, we outline how to configure your application to use either **SQLite** or **SQL Server** as the data store. These configurations provide the flexibility to choose a provider that best fits the application's specific needs and deployment context.

### SQLite Setup

SQLite is a lightweight, self-contained SQL database engine that is often used for local development, testing, or small-scale applications. To use SQLite with your Bonyan-based application, install the following package:

```bash
dotnet add package Bonyan.AspNetCore.Persistence.EntityFrameworkCore.Sqlite
```

Then, configure your `DbContext` within the infrastructure module:

```csharp
public override Task OnConfigureAsync(ModularityContext context)
{
    context.AddBonyanDbContext<CustomDbContext>(c =>
    {
        c.AddDefaultRepositories(true);
    });

    context.Services.Configure<EntityFrameworkDbContextOptions>(configuration =>
    {
        configuration.UseSqlite("Data Source=MyDatabase.db");
    });

    return base.OnConfigureAsync(context);
}
```

In this configuration, `AddBonyanDbContext` is used to register `CustomDbContext` and set up the repositories for the context. The configuration uses SQLite as the database provider, and the connection string specifies the location of the database file.

### SQL Server Setup

For enterprise-level applications or where more advanced database features are required, **SQL Server** is a suitable choice. To integrate SQL Server with your Bonyan-based application, use the following package:

```bash
dotnet add package Bonyan.AspNetCore.Persistence.EntityFrameworkCore.SqlServer
```

The configuration process for SQL Server is similar to SQLite, but you will specify a SQL Server connection string to connect to your desired database instance:

```csharp
context.Services.Configure<EntityFrameworkDbContextOptions>(configuration =>
{
    configuration.UseSqlServer("Server=MyServer;Database=MyDatabase;User Id=myusername;Password=mypassword;");
});
```

This configuration allows your application to interact with a SQL Server database. The connection string includes all necessary details such as server name, database name, user credentials, and other parameters required for establishing the connection.

## Best Practices and Considerations

- **Separation of Concerns**: Always separate domain logic from persistence concerns. Use repositories to abstract the database operations so that the domain remains focused on the core business rules.
- **Configuration Conventions**: Leverage `ConfigureByConvention` to automatically apply common configurations to entities. This ensures consistency in how entities are mapped to the database.
- **SQL Provider Selection**: Choose the appropriate SQL provider based on the scale and nature of your application. SQLite is often suitable for small applications or prototyping, whereas SQL Server is more appropriate for large-scale or production environments.
- **Migration Management**: Use Entity Framework Core migration tools to manage schema changes. Proper migration management can help maintain data integrity as your application evolves.

## Summary

The **Bonyan.AspNetCore.Persistence.EntityFrameworkCore** module provides a streamlined way to integrate Entity Framework Core with your domain-driven .NET Core applications. By utilizing **DbContexts** and **Repositories**, developers can efficiently implement data persistence while adhering to DDD principles. The module also supports a variety of SQL providers, including **SQLite** and **SQL Server**, providing flexibility to meet different application needs. The integration of this module ensures maintainability, scalability, and clear separation of concerns in the persistence layer of your applications.

Through the use of well-defined DbContexts and repository abstractions, you can keep the domain model clean and focused, allowing it to evolve independently of the underlying infrastructure. With support for common SQL providers and configuration conventions, this module is an essential tool for building robust, domain-driven .NET Core applications.

