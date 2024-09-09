# Function to get the last base version (without any pre-release or dev-release suffix)
function Get-LastBaseVersion {
    param (
        [array]$existingVersions
    )

    # Filter versions that do not have '-pre' or '-dev' suffixes
    $baseVersions = $existingVersions  | Select-Object -Last 1 
    $lastBase = $baseVersions -split '\-' 
    # Return the last (most recent) base version
    return $lastBase[0]
}

function Get-NextVersion {
    param (
        [string]$currentVersion,
        [string]$publishType,
        [array]$existingVersions
    )

    # Get the last base version from the existing versions
    $lastBaseVersion = Get-LastBaseVersion -existingVersions $existingVersions

    # Split the last base version into parts (e.g., major.minor.patch)
    $lastVersionParts = $lastBaseVersion -split '\.'

    # Handle Production (increment patch number)
    if ($publishType -eq 'Production') {
        $lastVersionParts[2] = [int]$lastVersionParts[2] + 1
        return "$($lastVersionParts[0]).$($lastVersionParts[1]).$($lastVersionParts[2])"
    }

    # Handle Prerelease
    if ($publishType -eq 'Prerelease') {
        # Check if there is already a pre-release associated with this base version
        $preReleaseVersion = $existingVersions | Where-Object { $_ -match "$lastBaseVersion-pre(\d+)" } | Select-Object -Last 1 

        # Only increment patch version if the currentVersion is Production, not another Prerelease or Development version
        if ($currentVersion -notmatch '-pre' -and $currentVersion -notmatch '-dev') {
            $lastVersionParts[2] = [int]$lastVersionParts[2] + 1
        }
        
        $next = "$($lastVersionParts[0]).$($lastVersionParts[1]).$($lastVersionParts[2])"

        if ($preReleaseVersion) {
            # Increment the pre-release version
            $preVersionNumber = [regex]::Match($preReleaseVersion, '-pre(\d+)').Groups[1].Value
            $nextPreVersion = [int]$preVersionNumber + 1
             
            return "$next-pre$nextPreVersion"
        } else {
            # Start from pre1 if no pre-release exists for this base version
            return "$next-pre1"
        }
    }

    # Handle Development
    if ($publishType -eq 'Development') {
        # Check if there is already a dev-release associated with this base version
        $devVersion = $existingVersions | Where-Object { $_ -match "$lastBaseVersion-dev(\d+)" } | Sort-Object | Select-Object -Last 1

        # Only increment patch version if the currentVersion is Production, not another Development or Prerelease version
        if ($currentVersion -notmatch '-pre' -and $currentVersion -notmatch '-dev') {
            $lastVersionParts[2] = [int]$lastVersionParts[2] + 1
        }
        
        $next = "$($lastVersionParts[0]).$($lastVersionParts[1]).$($lastVersionParts[2])"

        if ($devVersion) {
            # Increment the dev-release version
            $devVersionNumber = [regex]::Match($devVersion, '-dev(\d+)').Groups[1].Value
            $nextDevVersion = [int]$devVersionNumber + 1
            
            return "$next-dev$nextDevVersion"
        } else {
            # Start from dev1 if no dev-release exists for this base version
            return "$next-dev1"
        }
    }
}


# Function to extract all versions from the version file
function Get-ExistingVersionsFromFile {
    param (
        [string]$versionFilePath
    )

    # Read all lines from the version.txt file
    if (Test-Path $versionFilePath) {
        return Get-Content -Path $versionFilePath
    }

    # Return an empty array if the file doesn't exist
    return @()
}

# Function to prompt the user for a version with a default suggestion based on the publish type
function Get-VersionFromUser {
    param (
        [string]$currentVersion,
        [string]$publishType,
        [array]$existingVersions
    )

    # Get the suggested (next) version
    $nextVersion = Get-NextVersion -currentVersion $currentVersion -publishType $publishType -existingVersions $existingVersions

    # Prompt user for the version, showing the current version and default next version
    $promptMessage = "Current Version $currentVersion - Enter the new version (default: $nextVersion)"
    $userInputVersion = Read-Host $promptMessage

    # If the user provided input, return that; otherwise, return the default version
    if (-not [string]::IsNullOrEmpty($userInputVersion)) {
        return $userInputVersion
    } else {
        return $nextVersion
    }
}

# Function to update the version in the package.props file and version.txt file
function Update-VersionInPackageProps {
    param (
        [string]$packagePropsPath,
        [string]$publishType,
        [string]$versionFilePath
    )

    # Read the package.props content
    $propsContent = Get-Content -Path $packagePropsPath

    # Extract all existing versions from version.txt
    $existingVersions = Get-ExistingVersionsFromFile -versionFilePath $versionFilePath

    # Get the last base version (without any pre-release or dev-release)
    $lastBaseVersion = Get-LastBaseVersion -existingVersions $existingVersions

    # Get the new version from the user, using the selected publish type
    $newVersion = Get-VersionFromUser -currentVersion $lastBaseVersion -publishType $publishType -existingVersions $existingVersions

    # Show the version information
    Write-Host "Current Version: $lastBaseVersion"
    Write-Host "Next Version (suggested): $newVersion"

    # Replace the old version with the new version in the package.props file
    $newPropsContent = $propsContent -replace '<Version>.*?</Version>', "<Version>$newVersion</Version>"

    # Save the updated package.props file
    Set-Content -Path $packagePropsPath -Value $newPropsContent

    # Save the latest version to the version.txt file
    Save-LatestVersionToFile -versionFilePath $versionFilePath -newVersion $newVersion

    Write-Host "Updated version from $lastBaseVersion to $newVersion in $packagePropsPath."
}

# Function to save the latest version to the version file
function Save-LatestVersionToFile {
    param (
        [string]$versionFilePath,
        [string]$newVersion
    )

    # Append the new version to the version.txt file
    Add-Content -Path $versionFilePath -Value $newVersion
}

# Function to show a menu and let the user select a publish type using arrow keys
function Show-PublishMenu {
    # Menu options
    $menuOptions = @('Prerelease', 'Production', 'Development')
    $currentIndex = 0

    # Function to display the menu
    function DisplayMenu {
        Clear-Host
        Write-Host "Use the arrow keys to navigate and press Enter to select the publish type:"
        for ($i = 0; $i -lt $menuOptions.Count; $i++) {
            if ($i -eq $currentIndex) {
                # Highlight the current selection
                Write-Host " > $($menuOptions[$i])" -ForegroundColor Cyan
            } else {
                Write-Host "   $($menuOptions[$i])"
            }
        }
    }

    # Display the menu initially
    DisplayMenu

    # Loop to handle key presses
    while ($true) {
        $key = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

        switch ($key.VirtualKeyCode) {
            38 { # Up arrow key
                if ($currentIndex -gt 0) {
                    $currentIndex--
                    DisplayMenu
                }
            }
            40 { # Down arrow key
                if ($currentIndex -lt $menuOptions.Count - 1) {
                    $currentIndex++
                    DisplayMenu
                }
            }
            13 { # Enter key
                Clear-Host
                Write-Host "You selected: $($menuOptions[$currentIndex])" -ForegroundColor Green
                return $menuOptions[$currentIndex]
            }
        }
    }
}

# Function to publish the Dotnet project and push to NuGet
function Publish-DotnetProject {
    param (
        [string]$projectPath,
        [string]$publishType
    )

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
        # Prepare NuGet push command based on the publish type
        $nugetSource = 'https://api.nuget.org/v3/index.json'
        $nugetApiKey = 'oy2jcaidwrfsiom4l3ia3ng6x5yohh6ubjvdhjddmqk6dy'

        # Adjust command for Prerelease
       
            $nugetPushCommand = "dotnet nuget push `"$($nupkgFile.FullName)`" -s $nugetSource -k $nugetApiKey "
    

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

# Main script logic
function Main {
    # Save the initial directory
    $startingDirectory = Get-Location

    # Path to the package.props file and version.txt file
    $packagePropsPath = Join-Path -Path $startingDirectory -ChildPath 'package.props'
    $versionFilePath = Join-Path -Path $startingDirectory -ChildPath 'version.txt'

    # Show menu for publish type selection
    $publishType = Show-PublishMenu

    # Update the version in package.props and version.txt before publishing projects
    Update-VersionInPackageProps -packagePropsPath $packagePropsPath -publishType $publishType -versionFilePath $versionFilePath

    # Run tests
    dotnet test

    # Publish each project in the Src/Framework directory
    Set-Location -Path .\Framework\
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

    # Return to the starting directory
    Set-Location -Path $startingDirectory
}

# Start the script
Main
