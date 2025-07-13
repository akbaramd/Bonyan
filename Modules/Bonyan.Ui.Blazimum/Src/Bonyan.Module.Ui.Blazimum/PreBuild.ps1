# Define paths
$outputDir = "wwwroot/css"                     # Directory for global CSS output
$tailwindGlobalInput = "./Styles/tailwind.scss" # Main SCSS file for global Tailwind styles
$tailwindGlobalOutput = "$outputDir/blazimum.css" # Final global Tailwind CSS file

# Ensure output directory exists
if (!(Test-Path $outputDir)) {
    New-Item -ItemType Directory -Force -Path $outputDir | Out-Null
}

# Step 1: Compile Component-Specific .razor.scss Files
Write-Host "Processing component-specific SCSS files..."
$scssFiles = Get-ChildItem -Path . -Recurse -Filter *.razor.scss
foreach ($file in $scssFiles) {
    # Correctly rename .razor.scss to .razor.css
    $cssOutput = $file.FullName -replace '\.razor\.scss$', '.razor.css'

    Write-Host "Compiling SCSS: $($file.FullName) -> $cssOutput"
    try {
        # Compile .razor.scss to .razor.css
        npx sass $file.FullName $cssOutput --load-path ./Styles

        # Run Tailwind on the compiled CSS file
        Write-Host "Running Tailwind CSS: $cssOutput"
        npx tailwindcss -i $cssOutput -o $cssOutput

        Write-Host "Processed: $cssOutput"
    } catch {
        Write-Error "Failed to process $($file.FullName): $_"
        exit 1
    }
}

# Step 2: Compile Global Tailwind SCSS
Write-Host "Compiling global SCSS for Tailwind..."
try {
    # Compile global SCSS file to CSS
    npx sass $tailwindGlobalInput $tailwindGlobalOutput --load-path ./Styles

    # Run Tailwind on the global compiled CSS
    Write-Host "Running Tailwind CSS on global styles..."
    npx tailwindcss -i $tailwindGlobalOutput -o $tailwindGlobalOutput

    Write-Host "Global Tailwind CSS compiled successfully: $tailwindGlobalOutput"
} catch {
    Write-Error "Failed to process global SCSS: $_"
    exit 1
}

Write-Host "Pre-build process completed successfully."
