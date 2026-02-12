# Bonyan Upload Script
# Builds, packs NuGet packages (Framework, Modules, Template) and pushes to NuGet.org
# Requires: $env:NugetKey - NuGet API key for authentication

param(
    [switch]$PackOnly,   # Only pack, do not push
    [switch]$FrameworkOnly,
    [switch]$ModulesOnly,
    [switch]$TemplateOnly
)

$NugetSource = 'https://api.nuget.org/v3/index.json'

# Pack a .NET project and optionally push to NuGet
function Pack-AndPush-Project {
    param (
        [string]$ProjectPath,
        [string]$ProjectType,
        [string]$ProjectFile = $null,  # Optional: specific .csproj (e.g. Bonyan.Template.csproj)
        [bool]$Push = $true
    )

    $nugetApiKey = $env:NugetKey
    if ($Push -and -not $nugetApiKey) {
        Write-Host "NuGet API Key not found. Set the NugetKey environment variable to push packages." -ForegroundColor Red
        return $false
    }

    $initialDirectory = Get-Location

    try {
        Set-Location -Path $ProjectPath

        # Clean Release output
        $releaseFolder = Join-Path -Path $ProjectPath -ChildPath 'bin\Release'
        if (Test-Path -Path $releaseFolder -PathType Container) {
            Remove-Item -Path $releaseFolder -Recurse -Force -ErrorAction SilentlyContinue
        }

        # Pack the project (produces .nupkg)
        $packArgs = @('-c', 'Release')
        if ($ProjectFile) { $packArgs = @($ProjectFile) + $packArgs }
        Write-Host "Packing ${ProjectType}: $ProjectPath" -ForegroundColor Cyan
        $packResult = & dotnet pack @packArgs 2>&1
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Pack failed for $ProjectPath" -ForegroundColor Red
            Write-Host $packResult
            return $false
        }

        # Find .nupkg (may be in bin\Release or bin\Release\netX.X)
        $nupkgFiles = Get-ChildItem -Path $ProjectPath -Filter *.nupkg -Recurse -ErrorAction SilentlyContinue |
            Where-Object { $_.FullName -like "*\bin\Release*" }

        if (-not $nupkgFiles) {
            Write-Host "No .nupkg found in $ProjectPath\bin\Release" -ForegroundColor Yellow
            return $false
        }

        foreach ($nupkgFile in $nupkgFiles) {
            if ($Push) {
                Write-Host "Pushing $($nupkgFile.Name) to NuGet..." -ForegroundColor Cyan
                try {
                    dotnet nuget push $nupkgFile.FullName -s $NugetSource -k $nugetApiKey --skip-duplicate
                    Write-Host "Successfully pushed $($nupkgFile.Name)" -ForegroundColor Green
                } catch {
                    Write-Host "Error pushing $($nupkgFile.Name): $_" -ForegroundColor Red
                    return $false
                }
            } else {
                Write-Host "Packed $($nupkgFile.Name) (push skipped)" -ForegroundColor Green
            }
        }
        return $true
    } finally {
        Set-Location -Path $initialDirectory
    }
}

# Pack and push all Framework projects
function Publish-FrameworkProjects {
    param (
        [string]$FrameworkPath,
        [bool]$Push = $true
    )

    Write-Host "`n=== Packing Framework projects: $FrameworkPath ===" -ForegroundColor Yellow
    $initialDir = Get-Location
    Set-Location -Path $FrameworkPath

    $projects = Get-ChildItem -Directory | ForEach-Object {
        $csproj = Get-ChildItem -Path $_.FullName -Filter *.csproj -ErrorAction SilentlyContinue
        if ($csproj) { $csproj.DirectoryName }
    }

    foreach ($projectPath in $projects) {
        Pack-AndPush-Project -ProjectPath $projectPath -ProjectType "Framework" -Push $Push
    }

    Set-Location -Path $initialDir
}

# Pack and push all Module projects
function Publish-ModuleProjects {
    param (
        [string]$ModulesPath,
        [bool]$Push = $true
    )

    Write-Host "`n=== Packing Module projects: $ModulesPath ===" -ForegroundColor Yellow
    $initialDir = Get-Location
    Set-Location -Path $ModulesPath

    $moduleDirs = Get-ChildItem -Directory
    foreach ($moduleDir in $moduleDirs) {
        $srcPath = Join-Path -Path $moduleDir.FullName -ChildPath 'src'
        if (-not (Test-Path $srcPath)) {
            Write-Host "No src folder in $($moduleDir.Name)" -ForegroundColor Yellow
            continue
        }

        Write-Host "Processing module: $($moduleDir.Name)" -ForegroundColor Cyan
        $projects = Get-ChildItem -Path $srcPath -Directory | ForEach-Object {
            $csproj = Get-ChildItem -Path $_.FullName -Filter *.csproj -ErrorAction SilentlyContinue
            if ($csproj) { $csproj.DirectoryName }
        }

        foreach ($projectPath in $projects) {
            Pack-AndPush-Project -ProjectPath $projectPath -ProjectType "Module" -Push $Push
        }
    }

    Set-Location -Path $initialDir
}

# Pack and push the Bonyan dotnet new template
function Publish-Template {
    param (
        [string]$TemplatesPath,
        [bool]$Push = $true
    )

    Write-Host "`n=== Packing Bonyan Template ===" -ForegroundColor Yellow

    $templateCsproj = Join-Path -Path $TemplatesPath -ChildPath 'Bonyan.Template.csproj'
    if (-not (Test-Path $templateCsproj)) {
        Write-Host "Bonyan.Template.csproj not found at $TemplatesPath" -ForegroundColor Red
        return $false
    }

    return Pack-AndPush-Project -ProjectPath $TemplatesPath -ProjectFile 'Bonyan.Template.csproj' -ProjectType "Template" -Push $Push
}

# Main
function Main {
    $rootDir = if ($PSScriptRoot) { $PSScriptRoot } else { (Get-Location).Path }
    $push = -not $PackOnly

    if ($push -and -not $env:NugetKey) {
        Write-Host "Set NugetKey environment variable to push packages, or use -PackOnly to only pack." -ForegroundColor Red
        exit 1
    }

    # Restore (excludes Templates - they need packages published first)
    Write-Host "Restoring solution (Framework + Modules)..." -ForegroundColor Cyan
    $slnfPath = Join-Path $rootDir "Bonyan.Upload.slnf"
    if (Test-Path $slnfPath) {
        dotnet restore $slnfPath
    } else {
        dotnet restore $rootDir\Bonyan.sln
    }

    $frameworkPath = Join-Path $rootDir "Framework\Src"
    $modulesPath = Join-Path $rootDir "Modules"
    $templatesPath = Join-Path $rootDir "Templates"

    $runAll = -not $FrameworkOnly -and -not $ModulesOnly -and -not $TemplateOnly

    if ($runAll -or $FrameworkOnly) {
        if (Test-Path $frameworkPath) {
            Publish-FrameworkProjects -FrameworkPath $frameworkPath -Push $push
        } else {
            Write-Host "Framework path not found: $frameworkPath" -ForegroundColor Yellow
        }
    }

    if ($runAll -or $ModulesOnly) {
        if (Test-Path $modulesPath) {
            Publish-ModuleProjects -ModulesPath $modulesPath -Push $push
        } else {
            Write-Host "Modules path not found: $modulesPath" -ForegroundColor Yellow
        }
    }

    if ($runAll -or $TemplateOnly) {
        if (Test-Path $templatesPath) {
            Publish-Template -TemplatesPath $templatesPath -Push $push
        } else {
            Write-Host "Templates path not found: $templatesPath" -ForegroundColor Yellow
        }
    }

    Write-Host "`nDone." -ForegroundColor Green
}

Main
