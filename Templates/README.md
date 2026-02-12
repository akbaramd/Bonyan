# Bonyan Template (Enterprise / Microsoft style)

This solution template follows **enterprise** and **Microsoft developer** conventions for .NET solutions.

## Install as dotnet new template

Install from the Templates folder (or from NuGet once published):

```bash
dotnet new install d:\Projects\Bonyan2\Templates
```

## Create new project

**Option 1: MVC only** (Domain + Application + Infrastructure + MVC host)

```bash
dotnet new bonyan-mvc -n MyCompany -o MyCompany
```

**Option 2: Web API only** (Domain + Application + Infrastructure + Web API host)

```bash
dotnet new bonyan-api -n MyCompany -o MyCompany
```

**Option 3: Choose host at creation** (use `--host` parameter)

```bash
dotnet new bonyan -n MyCompany -o MyCompany --host Mvc
dotnet new bonyan -n MyCompany -o MyCompany --host Api
```

The `-n` / `--name` value replaces `BonyanTemplate` in filenames and code.

## Structure

| Project | Description |
|--------|-------------|
| **BonyanTemplate.Domain** | Domain layer: entities, repositories, domain events |
| **BonyanTemplate.Application** | Application layer: app services, DTOs, AutoMapper, workers |
| **BonyanTemplate.Infrastructure** | Data access: EF Core DbContext, repositories |
| **BonyanTemplate.Mvc** | Host: ASP.NET Core MVC with Razor views |
| **BonyanTemplate.WebApi** | Host: ASP.NET Core Web API, Swagger, health checks |

## Conventions

- **.editorconfig**: Microsoft-style C# formatting, naming, and code-style rules (file-scoped namespaces, braces, spacing).
- **XML documentation**: Public APIs are documented; `GenerateDocumentationFile` is enabled in all projects.
- **Analysis**: `AnalysisLevel = latest-recommended`, `EnforceCodeStyleInBuild = true` for consistent style at build time.
- **Naming**: Singular names for entities (e.g. `Author` not `Authors`).
- **Health checks** (Web API): `/health/ready` and `/health/live` endpoints for orchestration and monitoring.

## Running the projects

**MVC**
1. Set **BonyanTemplate.Mvc** (or your renamed project) as the startup project.
2. Run the application.

**Web API**
1. Set **BonyanTemplate.WebApi** as the startup project.
2. Run the application; Swagger is available at `/swagger`.
3. Use **BonyanTemplate.WebApi.http** for health and API requests.

## Configuration

- **appsettings.json**: Logging, Azure AD placeholders, and standard Microsoft-style sections.
- **appsettings.Development.json**: Development logging levels.
