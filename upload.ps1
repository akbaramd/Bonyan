# Function to publish the Dotnet project and push to NuGet
function Publish-DotnetProject {
    param (
        [string]$projectPath,
        [string]$publishType
    )

    # Retrieve NuGet API key from the environment variable
    $nugetApiKey = $env:NugetKey 

    if (-not $nugetApiKey) {
        Write-Host "NuGet API Key not found. Please set the NugetKey environment variable." -ForegroundColor Red
        return
    }

    # Save the current directory
    $initialDirectory = Get-Location

    # Change to the project directory
    Set-Location -Path $projectPath

    # Clean up the Release folder if it exists
    $releaseFolder = Join-Path -Path $projectPath -ChildPath 'bin\Release'
    if (Test-Path -Path $releaseFolder -PathType Container) {
        Remove-Item -Path $releaseFolder -Recurse -Force -ErrorAction SilentlyContinue
    }

    # Publish the project
    dotnet publish -c Release | Out-Null

    # Find the .nupkg file in the bin\Release folder
    Set-Location -Path $releaseFolder
    $nupkgFile = Get-ChildItem -Filter *.nupkg

    if ($nupkgFile) {
        # Prepare NuGet push command using the secret stored in the environment
        $nugetSource = 'https://api.nuget.org/v3/index.json'
        $nugetPushCommand = "dotnet nuget push `"$($nupkgFile.FullName)`" -s $nugetSource -k $nugetApiKey"

        # Log and execute the push command
        $logFile = Join-Path -Path $projectPath -ChildPath 'nuget_publish_log.txt'
        Add-Content -Path $logFile -Value "Executing NuGet push command for $($nupkgFile.FullName) at $(Get-Date)"
        Add-Content -Path $logFile -Value $nugetPushCommand

        try {
            Invoke-Expression -Command $nugetPushCommand

            Write-Host "Successfully uploaded $($nupkgFile.Name) to NuGet." -ForegroundColor Green
            Add-Content -Path $logFile -Value "Successfully uploaded $($nupkgFile.Name) to NuGet at $(Get-Date)."
        } catch {
            Write-Host "Error uploading $($nupkgFile.Name) to NuGet. $_" -ForegroundColor Red
            Add-Content -Path $logFile -Value "Error uploading $($nupkgFile.Name) to NuGet at $(Get-Date): $_"
        }
    } else {
        Write-Host "No .nupkg file found in $($releaseFolder)." -ForegroundColor Yellow
    }

    # Return to the initial directory
    Set-Location -Path $initialDirectory
}

# Function to iterate over a directory and publish projects
function Publish-ProjectsInDirectory {
    param (
        [string]$directoryPath,
        [string]$publishType
    )
    Write-Host "Start Publish project iside  $($directoryPath)." -ForegroundColor Yellow
    Set-Location -Path $directoryPath
    $subdirectories = Get-ChildItem -Directory

    foreach ($subdir in $subdirectories) {
        $subdirPath = $subdir.FullName
        Set-Location -Path $subdirPath

        # Check if there's a .csproj file in the directory
        $dotnetProject = Get-ChildItem -Filter *.csproj
        if ($dotnetProject) {
            Publish-DotnetProject -projectPath $subdirPath -publishType $publishType
        } else {
            Write-Host "No Dotnet project found in $($subdir.Name)." -ForegroundColor Yellow
        }
    }
}

# Main script logic
function Main {
    # Save the initial directory
    $startingDirectory = Get-Location

    # Run tests
    dotnet test
    Set-Location -Path $startingDirectory
    # Publish projects in the Framework directory
    $frameworkDirectory = ".\Framework\Src"
 
    if (Test-Path $frameworkDirectory) {
        Publish-ProjectsInDirectory -directoryPath $frameworkDirectory -publishType "Framework"
    } else {
        Write-Host "Framework directory not found." -ForegroundColor Yellow
    }
#     Set-Location -Path $startingDirectory
#     # Publish projects in the Modules directory
#     $modulesDirectory = ".\Modules"
#     if (Test-Path $modulesDirectory) {
#         Publish-ProjectsInDirectory -directoryPath $modulesDirectory -publishType "Modules"
#     } else {
#         Write-Host "Modules directory not found." -ForegroundColor Yellow
#     }

    # Return to the starting directory
    Set-Location -Path $startingDirectory
}

# Start the script
Main
