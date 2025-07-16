# Idempotent Migration Script Generator for Bonyan.Novino
# This script generates idempotent SQL scripts for safe database deployments

param(
    [string]$ProjectPath = "src/Bonyan.Novino.Infrastructure",
    [string]$StartupProject = "src/Bonyan.Ui.Novino.Web",
    [string]$OutputFile = "src/Bonyan.Novino.Infrastructure/Scripts/migration-script.sql",
    [string]$FromMigration = "",
    [string]$ToMigration = "",
    [switch]$Full,
    [switch]$Incremental,
    [switch]$Verbose
)

Write-Host "=== Bonyan.Novino Idempotent Migration Script Generator ===" -ForegroundColor Green

# Set the working directory to the solution root
$solutionRoot = Split-Path -Parent $PSScriptRoot
Set-Location $solutionRoot

# Build the EF command
$efCommand = "dotnet ef migrations script --idempotent --project $ProjectPath --startup-project $StartupProject --context AppDbContext --output $OutputFile"

# Add optional parameters
if ($FromMigration -and $ToMigration) {
    $efCommand += " --from $FromMigration --to $ToMigration"
    Write-Host "Generating incremental migration script from '$FromMigration' to '$ToMigration'..." -ForegroundColor Yellow
}
elseif ($Full) {
    Write-Host "Generating full migration script (all migrations)..." -ForegroundColor Yellow
}
elseif ($Incremental) {
    Write-Host "Generating incremental migration script (pending migrations)..." -ForegroundColor Yellow
}
else {
    Write-Host "Generating full migration script (all migrations)..." -ForegroundColor Yellow
}

if ($Verbose) {
    $efCommand += " --verbose"
}

# Execute the command
Write-Host "Executing: $efCommand" -ForegroundColor Cyan
Invoke-Expression $efCommand

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Idempotent migration script generated successfully: $OutputFile" -ForegroundColor Green
    
    # Show file info
    if (Test-Path $OutputFile) {
        $fileInfo = Get-Item $OutputFile
        Write-Host "File size: $($fileInfo.Length) bytes" -ForegroundColor Gray
        Write-Host "Created: $($fileInfo.CreationTime)" -ForegroundColor Gray
    }
    
    # Show usage instructions
    Write-Host ""
    Write-Host "=== Usage Instructions ===" -ForegroundColor Cyan
    Write-Host "1. Review the generated script: $OutputFile" -ForegroundColor White
    Write-Host "2. Test the script in a development environment first" -ForegroundColor White
    Write-Host "3. Execute the script in production:" -ForegroundColor White
    Write-Host "   sqlcmd -S your-server -d your-database -i $OutputFile" -ForegroundColor Yellow
    Write-Host "   OR" -ForegroundColor White
    Write-Host "   Invoke-Sqlcmd -InputFile $OutputFile -ServerInstance your-server -Database your-database" -ForegroundColor Yellow
}
else {
    Write-Host "✗ Failed to generate migration script" -ForegroundColor Red
    exit 1
}

Write-Host "=== Script generation completed ===" -ForegroundColor Green 