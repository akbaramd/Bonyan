# Update database using EF Core migrations.
# Run from solution root: .\Templates\BonyanTemplate.Infrastructure\Scripts\Update-Database.ps1

$ErrorActionPreference = "Stop"

# Ensure dotnet global tools are on PATH (required after .NET 10 install if ef is not found)
$dotnetTools = Join-Path $env:USERPROFILE ".dotnet\tools"
if (Test-Path $dotnetTools) { $env:PATH = "$dotnetTools;$env:PATH" }

$solutionRoot = if ($PSScriptRoot) {
    Split-Path (Split-Path (Split-Path $PSScriptRoot -Parent) -Parent) -Parent
} else {
    Resolve-Path "."
}
$infra = Join-Path $solutionRoot "Templates\BonyanTemplate.Infrastructure"
$startup = Join-Path $solutionRoot "Templates\BonyanTemplate.Mvc"

# Run from MVC project folder so design-time DbContext factory has a writable current directory (avoids SQLite Error 14)
Set-Location $startup
dotnet ef database update --project $infra --startup-project . --context BonyanTemplateDbContext
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
