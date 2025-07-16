# Database Migration Script for Bonyan.Novino
# This script handles database migrations in the Clean Architecture structure

param(
    [string]$ConnectionString = "Server=localhost\AHMADI,1433;Database=BonyanNovino;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True",
    [string]$ProjectPath = "src/Bonyan.Novino.Infrastructure",
    [string]$StartupProject = "src/Bonyan.Ui.Novino.Web",
    [switch]$Update,
    [switch]$Add,
    [switch]$Script,
    [string]$MigrationName = "",
    [string]$OutputFile = "src/Bonyan.Novino.Infrastructure/Scripts/migration-script.sql"
)

Write-Host "=== Bonyan.Novino Database Migration Script ===" -ForegroundColor Green

# Set the working directory to the solution root
$solutionRoot = Split-Path -Parent $PSScriptRoot
Set-Location $solutionRoot

if ($Add) {
    if ([string]::IsNullOrEmpty($MigrationName)) {
        Write-Host "Error: Migration name is required when adding a new migration" -ForegroundColor Red
        Write-Host "Usage: .\migrate-database.ps1 -Add -MigrationName 'InitialCreate'" -ForegroundColor Yellow
        exit 1
    }
    
    Write-Host "Adding new migration: $MigrationName" -ForegroundColor Yellow
    dotnet ef migrations add $MigrationName --project $ProjectPath --startup-project $StartupProject --context AppDbContext
}
elseif ($Update) {
    Write-Host "Updating database with latest migrations..." -ForegroundColor Yellow
    dotnet ef database update --project $ProjectPath --startup-project $StartupProject --context AppDbContext
}
elseif ($Script) {
    Write-Host "Generating idempotent migration script..." -ForegroundColor Yellow
    dotnet ef migrations script --idempotent --project $ProjectPath --startup-project $StartupProject --context AppDbContext --output $OutputFile
    Write-Host "Idempotent migration script generated: $OutputFile" -ForegroundColor Green
}
else {
    Write-Host "Available commands:" -ForegroundColor Cyan
    Write-Host "  -Add -MigrationName 'MigrationName'  : Add a new migration" -ForegroundColor White
    Write-Host "  -Update                              : Apply pending migrations to database" -ForegroundColor White
    Write-Host "  -Script [-OutputFile 'filename.sql'] : Generate idempotent migration script" -ForegroundColor White
    Write-Host ""
    Write-Host "Examples:" -ForegroundColor Cyan
    Write-Host "  .\migrate-database.ps1 -Add -MigrationName 'InitialCreate'" -ForegroundColor White
    Write-Host "  .\migrate-database.ps1 -Update" -ForegroundColor White
    Write-Host "  .\migrate-database.ps1 -Script" -ForegroundColor White
    Write-Host "  .\migrate-database.ps1 -Script -OutputFile 'production-migration.sql'" -ForegroundColor White
}

Write-Host "=== Migration script completed ===" -ForegroundColor Green 