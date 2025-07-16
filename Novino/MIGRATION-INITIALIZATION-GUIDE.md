# Migration Initialization Guide - Bonyan.Novino

This guide explains how to properly initialize and manage migrations in the Clean Architecture structure.

## 🏗️ Project Structure

```
src/
├── Bonyan.Novino.Domain/                    # Domain entities
├── Bonyan.Novino.Application/               # Application services
├── Bonyan.Novino.Infrastructure/            # Database & migrations
│   ├── Data/
│   │   ├── AppDbContext.cs                  # Database context
│   │   └── AppDbContextFactory.cs           # Design-time factory
│   ├── Migrations/                          # EF Core migrations
│   └── Scripts/                             # Generated SQL scripts
└── Bonyan.Ui.Novino.Web/                    # Web presentation layer
```

## 🚀 Initial Migration Setup

### Step 1: Ensure Clean Architecture Structure

Before creating migrations, ensure your Clean Architecture is properly set up:

1. **Domain Layer**: Contains entities and business logic
2. **Application Layer**: Contains services and use cases
3. **Infrastructure Layer**: Contains database context and migrations
4. **Web Layer**: Depends on Infrastructure layer

### Step 2: Verify Database Context

Ensure your `AppDbContext` is properly configured in the Infrastructure layer:

```csharp
// src/Bonyan.Novino.Infrastructure/Data/AppDbContext.cs
public class AppDbContext : BonDbContext<AppDbContext>
    , IBonIdentityManagementDbContext<User, Role>
    , IBonTenantDbContext
    , IBonNotificationManagementDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        modelBuilder.ConfigureIdentityManagement<User, Role>();
    }
}
```

### Step 3: Create Design-Time Factory

Ensure you have a design-time factory for migrations:

```csharp
// src/Bonyan.Novino.Infrastructure/Data/AppDbContextFactory.cs
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = "Server=localhost\\AHMADI,1433;Database=BonyanNovino;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
        
        optionsBuilder.UseSqlServer(connectionString);
        return new AppDbContext(optionsBuilder.Options);
    }
}
```

## 📝 Creating Your First Migration

### Method 1: Using PowerShell Script (Recommended)

```powershell
# Navigate to solution root
cd "C:\Users\ahmadi.UR-NEZAM\RiderProjects\Bonyan\Novin\Bonyan.Novino"

# Create initial migration
.\scripts\migrate-database.ps1 -Add -MigrationName "InitialCreate"
```

### Method 2: Using Entity Framework CLI

```bash
# Navigate to solution root
cd "C:\Users\ahmadi.UR-NEZAM\RiderProjects\Bonyan\Novin\Bonyan.Novino"

# Create initial migration
dotnet ef migrations add InitialCreate --project src/Bonyan.Novino.Infrastructure --startup-project src/Bonyan.Ui.Novino.Web --context AppDbContext
```

## 🔄 Applying Migrations

### Method 1: Using PowerShell Script (Recommended)

```powershell
# Apply migrations to database
.\scripts\migrate-database.ps1 -Update
```

### Method 2: Using Entity Framework CLI

```bash
# Apply migrations to database
dotnet ef database update --project src/Bonyan.Novino.Infrastructure --startup-project src/Bonyan.Ui.Novino.Web --context AppDbContext
```

## 📄 Generating SQL Scripts

### Generate Idempotent Script

```powershell
# Generate idempotent script (stored in Infrastructure/Scripts/)
.\scripts\migrate-database.ps1 -Script
```

### Generate Production Script

```powershell
# Generate production-ready script
.\scripts\generate-migration-script.ps1 -OutputFile "src/Bonyan.Novino.Infrastructure/Scripts/production-migration.sql"
```

## 🔍 Verifying Migrations

### List All Migrations

```bash
dotnet ef migrations list --project src/Bonyan.Novino.Infrastructure --startup-project src/Bonyan.Ui.Novino.Web
```

### Check Migration Status

```bash
# This will show which migrations are applied to the database
dotnet ef database update --project src/Bonyan.Novino.Infrastructure --startup-project src/Bonyan.Ui.Novino.Web --context AppDbContext --dry-run
```

## 🛠️ Troubleshooting

### Common Issues and Solutions

#### 1. "No database provider has been configured"

**Solution**: Ensure `Microsoft.EntityFrameworkCore.SqlServer` is referenced in Infrastructure project.

#### 2. "Unable to create an object of type 'AppDbContext'"

**Solution**: Verify `AppDbContextFactory` implements `IDesignTimeDbContextFactory<AppDbContext>`.

#### 3. "The type 'AppDbContext' cannot be used as type parameter"

**Solution**: Ensure AppDbContext inherits from the correct base class and implements required interfaces.

#### 4. Connection String Issues

**Solution**: Update connection string in `AppDbContextFactory.cs`:

```csharp
var connectionString = "Server=your-server;Database=your-database;Trusted_Connection=True;TrustServerCertificate=True";
```

## 📋 Complete Initialization Checklist

### ✅ Pre-Migration Setup
- [ ] Clean Architecture structure is in place
- [ ] AppDbContext is in Infrastructure layer
- [ ] AppDbContextFactory is configured
- [ ] All required packages are installed
- [ ] Project references are correct

### ✅ Migration Creation
- [ ] Initial migration created successfully
- [ ] Migration files are in Infrastructure/Migrations/
- [ ] No build errors in solution

### ✅ Database Update
- [ ] Migration applied to database
- [ ] Tables created successfully
- [ ] Migration history table exists

### ✅ Script Generation
- [ ] Idempotent script generated
- [ ] Script stored in Infrastructure/Scripts/
- [ ] Script can be executed safely

## 🎯 Best Practices

### 1. Always Use Infrastructure Project
- Migrations should be created in the Infrastructure layer
- Use Infrastructure project as target for EF commands
- Use Web project as startup project

### 2. Use Idempotent Scripts
- Always use `--idempotent` flag for production
- Test scripts in development first
- Use PowerShell scripts for consistency

### 3. Version Control
- Commit migration files to source control
- Don't commit generated SQL scripts (add to .gitignore)
- Document migration changes

### 4. Naming Conventions
- Use descriptive migration names: `AddUserTable`, `UpdateUserEmailField`
- Include date/version if needed: `V1_InitialCreate`

## 📚 Next Steps

After successful initialization:

1. **Add More Migrations**: When you need schema changes
2. **Seed Data**: Add initial data using migration service
3. **Production Deployment**: Use idempotent scripts for safe deployments
4. **Testing**: Test migrations in development environment

## 🔗 Related Files

- `scripts/migrate-database.ps1` - Main migration script
- `scripts/generate-migration-script.ps1` - Script generator
- `README-Migrations.md` - Detailed migration documentation
- `MIGRATION-SUMMARY.md` - Current migration status

---

**Note**: Always test migrations in a development environment before applying to production! 