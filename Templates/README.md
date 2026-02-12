# Bonyan Template (Enterprise / Microsoft style)

This solution template follows **enterprise** and **Microsoft developer** conventions for .NET solutions.

## Structure

| Project | Description |
|--------|-------------|
| **BonyanTemplate.Domain** | Domain layer: entities, repositories, domain events |
| **BonyanTemplate.Application** | Application layer: app services, DTOs, AutoMapper, workers |
| **BonyanTemplate.Infrastructure** | Data access: EF Core DbContext, repositories |
| **BonyanTemplate.WebApi** | Host: ASP.NET Core Web API, Swagger, health checks |

## Conventions

- **.editorconfig**: Microsoft-style C# formatting, naming, and code-style rules (file-scoped namespaces, braces, spacing).
- **XML documentation**: Public APIs are documented; `GenerateDocumentationFile` is enabled in all projects.
- **Analysis**: `AnalysisLevel = latest-recommended`, `EnforceCodeStyleInBuild = true` for consistent style at build time.
- **Naming**: Singular names for entities (e.g. `Author` not `Authors`).
- **Health checks**: `/health/ready` and `/health/live` endpoints for orchestration and monitoring.

## Running the Web API

1. Set **BonyanTemplate.WebApi** as the startup project.
2. Run the application; Swagger is available at `/swagger`.
3. Use **BonyanTemplate.WebApi.http** for health and API requests.

## Configuration

- **appsettings.json**: Logging, Azure AD placeholders, and standard Microsoft-style sections.
- **appsettings.Development.json**: Development logging levels.
