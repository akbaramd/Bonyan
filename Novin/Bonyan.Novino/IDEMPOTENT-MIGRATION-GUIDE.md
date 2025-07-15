# Idempotent Migration Guide - Bonyan.Novino

This guide shows you exactly how to work with idempotent migrations in the Clean Architecture structure.

## 🎯 What is Idempotent Migration?

**Idempotent** means the script can be run multiple times safely without causing errors. If objects already exist, the script skips them.

## 📁 Script Storage Location

All SQL scripts are now stored in the **Infrastructure layer**:
```
src/Bonyan.Novino.Infrastructure/Scripts/
├── migration-script.sql          # Generated idempotent script
├── production-migration.sql      # Production-ready script
└── README.md                     # Script documentation
```

## 🚀 Complete Migration Process

### Step 1: Create Migration
```powershell
# Create new migration
.\scripts\migrate-database.ps1 -Add -MigrationName "InitialCreate"
```

### Step 2: Generate Idempotent Script
```powershell
# Generate idempotent script (stored in Infrastructure/Scripts/)
.\scripts\migrate-database.ps1 -Script
```

### Step 3: Apply Migration (Choose One Method)

#### Method A: Using Entity Framework (Development)
```powershell
# Apply migration using EF Core
.\scripts\migrate-database.ps1 -Update
```

#### Method B: Using Idempotent Script (Production)
```powershell
# Execute the idempotent script directly
sqlcmd -S "your-server" -d "your-database" -i "src/Bonyan.Novino.Infrastructure/Scripts/migration-script.sql"
```

## 🔍 Understanding Idempotent Scripts

### What the Script Does

The generated idempotent script contains checks like this:

```sql
-- Check if migration already applied
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715093525_InitialCreate'
)
BEGIN
    -- Create table only if it doesn't exist
    CREATE TABLE [Roles] (
        [Id] nvarchar(450) NOT NULL,
        [Title] nvarchar(256) NOT NULL,
        [CanBeDeleted] bit NOT NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;
```

### Why This is Safe

1. **First Run**: Creates tables and records migration
2. **Second Run**: Checks migration history, skips creation
3. **Third Run**: Same as second run - no errors!

## 🛠️ Practical Examples

### Example 1: Fresh Database Setup

```powershell
# 1. Create migration
.\scripts\migrate-database.ps1 -Add -MigrationName "InitialCreate"

# 2. Generate script
.\scripts\migrate-database.ps1 -Script

# 3. Apply to fresh database
sqlcmd -S "localhost\AHMADI" -d "BonyanNovino" -i "src/Bonyan.Novino.Infrastructure/Scripts/migration-script.sql"
```

### Example 2: Production Deployment

```powershell
# 1. Generate production script
.\scripts\generate-migration-script.ps1 -OutputFile "src/Bonyan.Novino.Infrastructure/Scripts/production-migration.sql"

# 2. Deploy with backup
.\scripts\deploy-database.ps1 -ExecuteOnly -ServerInstance "prod-server" -DatabaseName "BonyanNovino" -Backup
```

### Example 3: Adding New Migration

```powershell
# 1. Add new migration
.\scripts\migrate-database.ps1 -Add -MigrationName "AddUserProfile"

# 2. Generate incremental script
.\scripts\generate-migration-script.ps1 -FromMigration "InitialCreate" -ToMigration "AddUserProfile" -OutputFile "src/Bonyan.Novino.Infrastructure/Scripts/incremental-migration.sql"

# 3. Apply
.\scripts\migrate-database.ps1 -Update
```

## 🔧 Troubleshooting

### Issue: "There is already an object named 'Roles' in the database"

**Solution**: Use the idempotent script instead of EF Core update:

```powershell
# ❌ This will fail if tables exist
.\scripts\migrate-database.ps1 -Update

# ✅ This will work safely
sqlcmd -S "localhost\AHMADI" -d "BonyanNovino" -i "src/Bonyan.Novino.Infrastructure/Scripts/migration-script.sql"
```

### Issue: Connection Problems

**Solution**: Check your connection string in `AppDbContextFactory.cs`:

```csharp
var connectionString = "Server=localhost\\AHMADI,1433;Database=BonyanNovino;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
```

## 📋 Migration Commands Reference

### PowerShell Scripts (Recommended)
```powershell
# Add migration
.\scripts\migrate-database.ps1 -Add -MigrationName "MigrationName"

# Update database
.\scripts\migrate-database.ps1 -Update

# Generate idempotent script
.\scripts\migrate-database.ps1 -Script

# Generate production script
.\scripts\generate-migration-script.ps1 -OutputFile "Scripts/production.sql"
```

### Entity Framework CLI
```bash
# List migrations
dotnet ef migrations list --project src/Bonyan.Novino.Infrastructure --startup-project src/Bonyan.Ui.Novino.Web

# Add migration
dotnet ef migrations add MigrationName --project src/Bonyan.Novino.Infrastructure --startup-project src/Bonyan.Ui.Novino.Web

# Generate idempotent script
dotnet ef migrations script --idempotent --project src/Bonyan.Novino.Infrastructure --startup-project src/Bonyan.Ui.Novino.Web --output src/Bonyan.Novino.Infrastructure/Scripts/migration.sql
```

## 🎯 Best Practices

### 1. Always Use Infrastructure Scripts Directory
- Scripts are automatically stored in `src/Bonyan.Novino.Infrastructure/Scripts/`
- This maintains Clean Architecture separation
- Scripts are version-controlled with the Infrastructure layer

### 2. Use Idempotent Scripts for Production
- Always use `--idempotent` flag
- Test scripts in development first
- Use PowerShell scripts for consistency

### 3. Migration Safety
- Always backup before applying migrations
- Use incremental scripts for large deployments
- Test on production-like data

### 4. Script Naming
- Use descriptive names: `migration-script.sql`, `production-migration.sql`
- Include timestamps for versioning: `migration-20250715-093525.sql`

## 🔄 Current Status

✅ **Migration Created**: `20250715093525_InitialCreate`
✅ **Idempotent Script Generated**: `src/Bonyan.Novino.Infrastructure/Scripts/migration-script.sql`
✅ **Script Size**: 9.6KB (345 lines)
✅ **Script Location**: Infrastructure layer (Clean Architecture compliant)

## 🚀 Next Steps

1. **Test the Script**: Run it on a test database
2. **Apply to Production**: Use the idempotent script for safe deployment
3. **Add More Migrations**: Follow the same process for schema changes
4. **Automate**: Use the PowerShell scripts in CI/CD pipelines

---

**Key Takeaway**: Idempotent scripts are your safety net! They can be run multiple times without errors, making database deployments much safer. 