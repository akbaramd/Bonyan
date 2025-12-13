# Web API Demo - Bonyan Modularity System

This is a demo Web API application showcasing the **Bonyan Modularity System**. It demonstrates:

- ✅ Modular architecture with separate modules
- ✅ Module dependencies using `[DependsOn]` attributes
- ✅ Module lifecycle (PreConfigure, Configure, PostConfigure, PreInitialize, Initialize, PostInitialize)
- ✅ Configuration management per module
- ✅ Service registration within modules
- ✅ Clean separation of concerns

## 🏗️ Architecture

### Modules

1. **WebApiDemoModule** (Root Module)
   - Root module that depends on `BonAspNetCoreModule`
   - Configures core services (Controllers, Swagger)
   - Defines application-level options

2. **ProductModule**
   - Demonstrates feature-specific module
   - Registers product services and repository
   - Has its own configuration options

3. **SwaggerModule**
   - Configures Swagger/OpenAPI documentation
   - Conditionally enabled based on configuration

### Module Dependencies

```
WebApiDemoModule
  └── DependsOn: BonAspNetCoreModule

ProductModule
  └── DependsOn: WebApiDemoModule

SwaggerModule
  └── DependsOn: WebApiDemoModule
```

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 or Rider (or any .NET IDE)

### Running the Application

1. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

2. **Run the application:**
   ```bash
   dotnet run --project WebApi.Demo
   ```

3. **Access the API:**
   - Swagger UI: `http://localhost:5000/swagger`
   - API Info: `http://localhost:5000/api/info`
   - Products API: `http://localhost:5000/api/product`

## 📋 API Endpoints

### Products API

- `GET /api/product` - Get all products
- `GET /api/product/{id}` - Get product by ID
- `POST /api/product` - Create a new product
- `PUT /api/product/{id}` - Update a product
- `DELETE /api/product/{id}` - Delete a product

### Info API

- `GET /api/info` - Get application and module information

## 🔧 Configuration

Configuration is managed in `appsettings.json`:

```json
{
  "WebApiDemo": {
    "ApplicationName": "Web API Demo",
    "Version": "1.0.0",
    "EnableSwagger": true
  },
  "ProductModule": {
    "MaxProductsPerPage": 100,
    "EnableCaching": true,
    "CacheExpirationMinutes": 5
  }
}
```

## 📦 Module Structure

Each module follows this pattern:

```csharp
[DependsOn(typeof(OtherModule))]
public class MyModule : BonModule
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
    {
        // Register services
        context.Services.AddScoped<IMyService, MyService>();
        
        // Configure options
        context.ConfigureOptions<MyModuleOptions>(options => {
            options.SomeSetting = "value";
        });
        
        return ValueTask.CompletedTask;
    }
}
```

## 🎯 Key Features Demonstrated

1. **Module Lifecycle**: All 6 lifecycle phases are available
2. **Dependency Management**: Automatic dependency resolution and ordering
3. **Configuration**: Per-module configuration with options pattern
4. **Service Registration**: Services registered within module context
5. **Clean Architecture**: Separation of concerns with modules

## 📚 Learn More

- [Bonyan Modularity System Documentation](../../Framework/Src/Bonyan/Bonyan/Modularity/BONYAN_MODULARITY_SYSTEM.md)
- [Microkernel Architecture Rules](../../Framework/Src/Bonyan/Bonyan/Modularity/MICROKERNEL_RULES.md)
- [Fluent API Proposal](../../Framework/Src/Bonyan/Bonyan/Modularity/FLUENT_API_PROPOSAL.md)

