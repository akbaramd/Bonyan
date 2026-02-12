# Add a new EF Core migration.
# Run from solution root: .\Templates\BonyanTemplate.Infrastructure\Scripts\Add-Migration.ps1 -Name MigrationName
# Example: .\Templates\BonyanTemplate.Infrastructure\Scripts\Add-Migration.ps1 -Name InitialCreate

param(
    [Parameter(Mandatory=$true)]
    [string]$Name
)

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

Set-Location $solutionRoot
dotnet ef migrations add $Name --project $infra --startup-project $startup --context BonyanTemplateDbContext
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
