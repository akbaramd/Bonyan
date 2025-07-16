# Database Migrations in Clean Architecture

This document explains how to handle database migrations in the Bonyan.Novino Clean Architecture structure.

## Project Structure

```
src/
├── Bonyan.Novino.Domain/           # Domain entities and business logic
├── Bonyan.Novino.Application/      # Application services and use cases
├── Bonyan.Novino.Infrastructure/   # Database context, migrations, and external services
└── Bonyan.Ui.Novino.Web/          # Web presentation layer
```

## Migration Setup

### 1. Infrastructure Layer
- **AppDbContext**: Located in `Bonyan.Novino.Infrastructure/Data/AppDbContext.cs`
- **AppDbContextFactory**: Design-time factory for migrations
- **DatabaseMigrationService**: Service for handling migrations programmatically

### 2. Migration Tools
- **PowerShell Script**: `scripts/migrate-database.ps1` for easy migration management
- **Entity Framework Tools**: Configured in Infrastructure project

## How to Use Migrations

### Using PowerShell Script (Recommended)

1. **Add a new migration:**
   ```powershell
   .\scripts\migrate-database.ps1 -Add -MigrationName "InitialCreate"
   ```

2. **Update database with pending migrations:**
   ```powershell
   .\scripts\migrate-database.ps1 -Update
   ```

3. **Generate idempotent migration script:**
   ```powershell
   .\scripts\migrate-database.ps1 -Script
   ```

### Using Idempotent Migration Scripts (Production Recommended)

1. **Generate idempotent script:**
   ```powershell
   .\scripts\generate-migration-script.ps1 -OutputFile "production-migration.sql"
   ```

2. **Generate incremental script:**
   ```powershell
   .\scripts\generate-migration-script.ps1 -FromMigration "20231201_InitialCreate" -ToMigration "20231215_AddUserTable" -OutputFile "incremental-migration.sql"
   ```

3. **Deploy with backup and safety checks:**
   ```powershell
   # Step 1: Generate the script
   .\scripts\deploy-database.ps1 -GenerateOnly -ScriptFile "deployment-script.sql"
   
   # Step 2: Execute the script (with backup)
   .\scripts\deploy-database.ps1 -ExecuteOnly -ServerInstance "localhost\AHMADI" -DatabaseName "BonyanNovino" -Backup
   ```

### Using Entity Framework CLI

1. **Add a new migration:**
   ```bash
   dotnet ef migrations add InitialCreate --project src/Bonyan.Novino.Infrastructure --startup-project src/Bonyan.Ui.Novino.Web --context AppDbContext
   ```

2. **Update database:**
   ```bash
   dotnet ef database update --project src/Bonyan.Novino.Infrastructure --startup-project src/Bonyan.Ui.Novino.Web --context AppDbContext
   ```

3. **Remove last migration:**
   ```bash
   dotnet ef migrations remove --project src/Bonyan.Novino.Infrastructure --startup-project src/Bonyan.Ui.Novino.Web --context AppDbContext
   ```

## Migration Best Practices

### 1. Always Use Infrastructure Project
- Migrations should be created in the Infrastructure layer
- Use the Infrastructure project as the target for EF commands
- Use the Web project as the startup project

### 2. Use Idempotent Scripts for Production
- Always use `--idempotent` flag for production deployments
- Idempotent scripts can be run multiple times safely
- Test scripts in development environment first
- Use the deployment script with backup options

### 3. Migration Safety
- Always backup databases before applying migrations
- Use incremental scripts for large deployments
- Test migrations on production-like data
- Have a rollback plan ready

### 2. Migration Naming
- Use descriptive names: `AddUserTable`, `UpdateUserEmailField`, etc.
- Include the date or version if needed: `V1_InitialCreate`

### 3. Database Connection
- Connection string is configured in `AppDbContextFactory.cs`
- Update the connection string in the factory for different environments

### 4. Seeding Data
- Use the `DatabaseMigrationService` for programmatic seeding
- Seed data should be in the Infrastructure layer
- Call seeding methods in the module's `OnApplicationAsync` method

## Troubleshooting

### Common Issues

1. **"No database provider has been configured"**
   - Ensure `Microsoft.EntityFrameworkCore.SqlServer` is referenced in Infrastructure project
   - Check that `AppDbContextFactory` has the correct connection string

2. **"Unable to create an object of type 'AppDbContext'"**
   - Verify `AppDbContextFactory` implements `IDesignTimeDbContextFactory<AppDbContext>`
   - Check that the factory is in the same assembly as the DbContext

3. **"The type 'AppDbContext' cannot be used as type parameter"**
   - Ensure AppDbContext inherits from the correct base class
   - Check that all required interfaces are implemented

### Migration Commands Reference

```bash
# List migrations
dotnet ef migrations list --project src/Bonyan.Novino.Infrastructure --startup-project src/Bonyan.Ui.Novino.Web

# Generate idempotent SQL script (recommended for production)
dotnet ef migrations script --idempotent --project src/Bonyan.Novino.Infrastructure --startup-project src/Bonyan.Ui.Novino.Web --output migration-script.sql

# Generate incremental SQL script
dotnet ef migrations script --idempotent --from InitialCreate --to AddUserTable --project src/Bonyan.Novino.Infrastructure --startup-project src/Bonyan.Ui.Novino.Web

# Drop database
dotnet ef database drop --project src/Bonyan.Novino.Infrastructure --startup-project src/Bonyan.Ui.Novino.Web
```

## Environment-Specific Configuration

### Development
- Uses local SQL Server instance
- Connection string: `Server=localhost\AHMADI,1433;Database=BonyanNovino;...`

### Production
- Update connection string in `AppDbContextFactory`
- Use environment variables for sensitive data
- Consider using user secrets for development

## Integration with Clean Architecture

### Domain Layer
- Contains entities and domain logic
- No direct database dependencies

### Application Layer
- Contains business logic and use cases
- No direct database dependencies

### Infrastructure Layer
- Contains database context and migrations
- Implements repository interfaces from Application layer

### Web Layer
- Contains controllers and views
- Depends on Infrastructure layer for database access 