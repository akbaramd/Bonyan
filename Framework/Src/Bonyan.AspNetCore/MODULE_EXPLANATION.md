# Bonyan.AspNetCore Module - Complete Explanation

## 📖 What This Module Does

The **Bonyan.AspNetCore** module provides integration between the Bonyan Modularity System and ASP.NET Core. It enables building modular web applications using the microkernel architecture pattern.

---

## 🏗️ Architecture Overview

### Core Components

1. **BonyanApplication** - Entry point for creating modular web applications
2. **BonyanApplicationBuilder** - Fluent builder for configuring the application
3. **BonAspNetCoreModule** - Root module that configures ASP.NET Core services
4. **WebBonModularityApplication** - Extends modularity system for web applications
5. **BonWebModule** - Base class for web-specific modules

### Module Structure

```
Bonyan.AspNetCore
├── Bonyan/
│   ├── AspNetCore/          # Core ASP.NET Core integration
│   │   ├── BonAspNetCoreModule.cs
│   │   ├── Security/        # Security middleware
│   │   └── Tracing/         # Correlation ID middleware
│   ├── ExceptionHandling/   # Global exception handling
│   ├── Modularity/          # Web module extensions
│   └── UnitOfWork/          # Unit of Work middleware
└── Microsoft/
    ├── AspNetCore/
    │   ├── Builder/         # Application builder
    │   └── Routing/         # Endpoint routing
    └── Extensions/
        └── DependencyInjection/  # Service registration
```

---

## 🔄 How It Works

### 1. Application Creation Flow

```csharp
// Step 1: Create builder
var builder = BonyanApplication.CreateModularBuilder<WebApiDemoModule>(
    serviceName: "web-api-demo");

// Step 2: Configure services (fluent API)
builder.Services.AddControllers();

// Step 3: Build application
var app = await builder.BuildAsync(context => {
    // Configure middleware
    context.Application.UseSwagger();
});

// Step 4: Run
await app.RunAsync();
```

### 2. Module Lifecycle

**Configuration Phases** (Before ServiceProvider build):
1. `OnPreConfigureAsync` - Early setup
2. `OnConfigureAsync` - Main configuration
3. `OnPostConfigureAsync` - Final adjustments

**Initialization Phases** (After ServiceProvider build):
1. `OnPreInitializeAsync` - Early initialization
2. `OnInitializeAsync` - Main initialization
3. `OnPostInitializeAsync` - Final initialization

**Web Application Phases** (After app build):
1. `OnPreApplicationAsync` - Before middleware pipeline
2. `OnApplicationAsync` - Middleware configuration
3. `OnPostApplicationAsync` - After middleware, endpoint configuration

### 3. Middleware Pipeline

The module configures middleware in this order:
1. Static Files
2. HTTPS Redirection
3. Antiforgery
4. Routing
5. Exception Handling (optional)
6. Correlation ID
7. Unit of Work
8. Claims Mapping
9. Endpoints

---

## 🎯 Key Features

### 1. Modular Application Builder
- Fluent API for configuration
- Automatic module discovery
- Dependency resolution
- Plugin support

### 2. Web Module Support
- Extends base module system for web
- Additional lifecycle phases for middleware
- Endpoint configuration

### 3. Middleware Integration
- Exception handling
- Correlation ID tracking
- Unit of Work management
- Claims mapping

### 4. Endpoint Routing
- Configurable endpoint registration
- Module information endpoint (`/bonyan/modules`)
- Extensible endpoint configuration

---

## 📝 Usage Example

```csharp
// Program.cs
var builder = BonyanApplication.CreateModularBuilder<MyWebModule>(
    serviceName: "my-service",
    creationContext: ctx => {
        // Configure plugins
        ctx.PlugInSources.AddFolder("Plugins");
    });

// Configure services
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Build and run
var app = await builder.BuildAsync(context => {
    context.Application.UseSwagger();
    context.Application.UseSwaggerUI();
});

await app.RunAsync();
```

---

## 🔧 Module Responsibilities

### BonAspNetCoreModule
- Configures core ASP.NET Core services
- Registers middleware
- Sets up endpoint routing
- Configures exception handling

### WebBonModularityApplication
- Extends base modularity for web
- Manages web module lifecycle
- Executes web application phases

### BonWebModule
- Base class for web modules
- Provides web-specific lifecycle hooks
- Extends base module functionality

---

## ⚠️ Current Limitations

1. **No Fluent API** - Configuration is verbose
2. **Async Issues** - Potential deadlocks
3. **Missing Features** - No health checks, limited CORS support
4. **Hardcoded Dependencies** - Autofac, Newtonsoft.Json
5. **Incomplete Builder** - Some methods throw NotImplementedException

See `ARCHITECTURE_ANALYSIS_REPORT.md` for complete list of issues.

---

## 🚀 Future Improvements

1. **Fluent API** - Chainable configuration methods
2. **Better Error Handling** - Proper HTTP status codes
3. **Health Checks** - Built-in health check endpoints
4. **Observability** - Structured logging, metrics
5. **Flexibility** - Support multiple DI containers, JSON libraries

