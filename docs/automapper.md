# Bonyan AutoMapper Module Guide

The `Bonyan.AutoMapperModule` is an essential component of the **Bonyan** framework, providing seamless integration with **AutoMapper** to facilitate the conversion of domain entities into Data Transfer Objects (DTOs) and vice versa. This module makes it easy to handle complex object mappings across different layers of your application.

## Installing the AutoMapper Module

To begin using the AutoMapper functionality, you need to add the `Bonyan.AutoMapper` package to your project. You can install it via the .NET CLI as follows:

```bash
dotnet add package Bonyan.AutoMapper
```

## Adding the AutoMapper Module

To use this module, you need to depend on `BonyanAutoMapperModule` within your target module. This makes all AutoMapper functionality available for mapping objects within your application.

```csharp
public class YourTargetModule : Module
{
    public YourTargetModule()
    {
        DependOn<BonyanAutoMapperModule>();
    }
}
```

By adding `BonyanAutoMapperModule`, you gain access to the AutoMapper capabilities that are essential for transforming your domain entities into more easily consumable formats such as DTOs.

## Configuring AutoMapper Profiles

In the Bonyan architecture, AutoMapper profiles can be easily added to the `BonyanAutoMapperOptions` configuration. A profile is used to define how objects are mapped to each other. You can create your own profiles to suit your application's specific needs.

Here’s an example of how you can add a custom AutoMapper profile:

```csharp
public override Task OnConfigureAsync(ServiceConfigurationContext context)
{
    context.Services.AddTransient<ITenantApplicationService, TenantApplicationService>();

    context.Services.Configure<BonyanAutoMapperOptions>(c =>
    {
        c.AddProfile<TenantProfile>(true);
    });

    return base.OnConfigureAsync(context);
}
```

In this example:
- **`OnConfigureAsync`**: This method is used to configure services during the startup of the module.
- **`AddProfile<TenantProfile>(true)`**: Adds a custom AutoMapper profile named `TenantProfile` to the Bonyan configuration. The boolean value indicates whether to validate the profile configuration when it’s added.

### Example Profile: `TenantProfile`

Here's an example of how a simple AutoMapper profile might look like:

```csharp
public class TenantProfile : Profile
{
    public TenantProfile()
    {
        CreateMap<Tenant, TenantDto>();
        CreateMap<TenantDto, Tenant>();
    }
}
```
In this `TenantProfile` example:
- **`CreateMap<Tenant, TenantDto>()`**: Defines a mapping between the `Tenant` entity and the `TenantDto`. This mapping allows AutoMapper to transform a `Tenant` object into its DTO representation and vice versa.

## Creating Application Services with AutoMapper

When using `BonyanLayerApplicationModule` with `BonyanAutoMapperModule`, you can easily convert entities to DTOs in your application services. The `ApplicationService` class provides a property `Mapper` which is a pre-configured AutoMapper instance that you can use directly.

Here’s an example that demonstrates how you can use AutoMapper in an application service:

```csharp
public class TenantAppService : ApplicationService
{
    private ITenantRepository _tenantRepository => LazyServiceProvider.LazyGetRequiredService<ITenantRepository>();

    public async Task<TenantDto> GetTenantByIdAsync(int id)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id);
        return Mapper.Map<TenantDto>(tenant);
    }
}
```

### Explanation

- **`TenantAppService`**: Inherits from `ApplicationService`, thereby gaining access to `Mapper`.
- **`GetTenantByIdAsync`**: This method retrieves a `Tenant` entity from the repository and then uses the `Mapper` property to convert it to a `TenantDto`. This simplifies the transformation logic and ensures consistency across the application.

## Summary

The `BonyanAutoMapperModule` is a powerful addition to the Bonyan framework, simplifying the conversion between domain entities and DTOs. By integrating AutoMapper, developers can significantly reduce the boilerplate code required to transform data across different layers of the application.

To use this module effectively:
1. **Install the package** via the `.NET CLI`.
2. **Depend on the `BonyanAutoMapperModule`** in your target module.
3. **Add custom profiles** as needed to define the mappings for your application's specific entities and DTOs.

By following these steps, you can leverage AutoMapper to streamline data transformations and ensure a consistent approach to object mapping throughout your application.

