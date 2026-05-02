# Define the project root
$root = "Web"

# Define the folder and file mapping based on the screenshot
$structure = @{
    "$root/Common"          = @()
    "$root/Dto's"           = @()
    "$root/EnumsHelper"     = @("EnumResolver.cs")
    "$root/EnumsHelper/Booking"      = @()
    "$root/EnumsHelper/Dispute"      = @()
    "$root/EnumsHelper/Notification" = @()
    "$root/EnumsHelper/Property"     = @()
    "$root/EnumsHelper/User"         = @()
    "$root/HelpersFactory"  = @()
    "$root/Responses"       = @()
}

# Execution logic
foreach ($path in $structure.Keys) {
    # Create the folder path if it doesn't exist
    if (!(Test-Path $path)) {
        New-Item -Path $path -ItemType Directory | Out-Null
        Write-Host "Created Folder: $path" -ForegroundColor Cyan
    }

    # Create the specific files for that folder
    foreach ($file in $structure[$path]) {
        $filePath = Join-Path $path $file
        if (!(Test-Path $filePath)) {
            New-Item -Path $filePath -ItemType File | Out-Null
            Write-Host "  -> Created File: $file" -ForegroundColor Gray
        }
    }
}

Write-Host "`nShared project structure created!" -ForegroundColor Green