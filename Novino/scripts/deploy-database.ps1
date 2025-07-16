# Database Deployment Script for Bonyan.Novino
# This script uses idempotent migrations for safe database deployments

param(
    [Parameter(Mandatory=$true)]
    [string]$ServerInstance,
    
    [Parameter(Mandatory=$true)]
    [string]$DatabaseName,
    
    [string]$ProjectPath = "src/Bonyan.Novino.Infrastructure",
    [string]$StartupProject = "src/Bonyan.Ui.Novino.Web",
    [string]$ScriptFile = "src/Bonyan.Novino.Infrastructure/Scripts/deployment-script.sql",
    [switch]$GenerateOnly,
    [switch]$ExecuteOnly,
    [switch]$Backup,
    [string]$BackupPath = "",
    [switch]$Verbose,
    [switch]$Force
)

Write-Host "=== Bonyan.Novino Database Deployment Script ===" -ForegroundColor Green

# Set the working directory to the solution root
$solutionRoot = Split-Path -Parent $PSScriptRoot
Set-Location $solutionRoot

# Validate parameters
if (-not $GenerateOnly -and -not $ExecuteOnly) {
    Write-Host "Either -GenerateOnly or -ExecuteOnly must be specified" -ForegroundColor Red
    exit 1
}

# Function to test database connection
function Test-DatabaseConnection {
    param([string]$Server, [string]$Database)
    
    try {
        $connectionString = "Server=$Server;Database=$Database;Trusted_Connection=True;TrustServerCertificate=True"
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        $connection.Close()
        return $true
    }
    catch {
        Write-Host "Failed to connect to database: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Function to create backup
function Backup-Database {
    param([string]$Server, [string]$Database, [string]$BackupPath)
    
    if ([string]::IsNullOrEmpty($BackupPath)) {
        $BackupPath = "backups"
    }
    
    if (-not (Test-Path $BackupPath)) {
        New-Item -ItemType Directory -Path $BackupPath -Force | Out-Null
    }
    
    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $backupFile = Join-Path $BackupPath "$Database`_$timestamp.bak"
    
    Write-Host "Creating database backup: $backupFile" -ForegroundColor Yellow
    
    $backupQuery = "BACKUP DATABASE [$Database] TO DISK = '$backupFile' WITH FORMAT, INIT, NAME = N'$Database-Full Database Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10"
    
    try {
        sqlcmd -S $Server -Q $backupQuery
        Write-Host "✓ Database backup created successfully" -ForegroundColor Green
        return $backupFile
    }
    catch {
        Write-Host "✗ Failed to create database backup: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# Main deployment logic
if ($GenerateOnly) {
    Write-Host "Generating idempotent migration script..." -ForegroundColor Yellow
    
    $efCommand = "dotnet ef migrations script --idempotent --project $ProjectPath --startup-project $StartupProject --context AppDbContext --output $ScriptFile"
    
    if ($Verbose) {
        $efCommand += " --verbose"
    }
    
    Write-Host "Executing: $efCommand" -ForegroundColor Cyan
    Invoke-Expression $efCommand
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Idempotent migration script generated: $ScriptFile" -ForegroundColor Green
        
        # Show script preview
        if (Test-Path $ScriptFile) {
            $fileInfo = Get-Item $ScriptFile
            Write-Host "File size: $($fileInfo.Length) bytes" -ForegroundColor Gray
            
            if ($Verbose) {
                Write-Host "Script preview (first 10 lines):" -ForegroundColor Cyan
                Get-Content $ScriptFile | Select-Object -First 10 | ForEach-Object { Write-Host "  $_" -ForegroundColor Gray }
            }
        }
    }
    else {
        Write-Host "✗ Failed to generate migration script" -ForegroundColor Red
        exit 1
    }
}

if ($ExecuteOnly) {
    if (-not (Test-Path $ScriptFile)) {
        Write-Host "Script file not found: $ScriptFile" -ForegroundColor Red
        Write-Host "Generate the script first using: .\deploy-database.ps1 -GenerateOnly" -ForegroundColor Yellow
        exit 1
    }
    
    Write-Host "Testing database connection..." -ForegroundColor Yellow
    if (-not (Test-DatabaseConnection -Server $ServerInstance -Database $DatabaseName)) {
        Write-Host "Cannot connect to database. Please check your connection parameters." -ForegroundColor Red
        exit 1
    }
    
    if ($Backup) {
        $backupFile = Backup-Database -Server $ServerInstance -Database $DatabaseName -BackupPath $BackupPath
        if (-not $backupFile) {
            if (-not $Force) {
                Write-Host "Backup failed. Use -Force to continue without backup." -ForegroundColor Red
                exit 1
            }
        }
    }
    
    Write-Host "Executing migration script on $ServerInstance\$DatabaseName..." -ForegroundColor Yellow
    
    try {
        $result = sqlcmd -S $ServerInstance -d $DatabaseName -i $ScriptFile -b
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Database migration executed successfully" -ForegroundColor Green
        }
        else {
            Write-Host "✗ Database migration failed" -ForegroundColor Red
            Write-Host "Exit code: $LASTEXITCODE" -ForegroundColor Red
            exit 1
        }
    }
    catch {
        Write-Host "✗ Error executing migration script: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
}

Write-Host "=== Deployment completed ===" -ForegroundColor Green 