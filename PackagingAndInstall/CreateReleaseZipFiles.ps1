
function WriteBuildStep([string] $buildStepName)
{
    # Create separator line string

    [int] $consoleWidth = $host.UI.RawUI.BufferSize.Width - 1

    [System.Text.StringBuilder] $separatorLineStringBuilder = New-Object -TypeName "System.Text.StringBuilder"

    for ($charIndex = 0; $charIndex -lt $consoleWidth; $charIndex++)
    {
        $separatorLineStringBuilder.Append("-") | Out-Null
    }

    # Output build step
    Write-Host
    Write-Host -ForegroundColor Yellow -Object $separatorLineStringBuilder
    Write-Host -ForegroundColor Yellow -Object "    $buildStepName"
    Write-Host -ForegroundColor Yellow -Object $separatorLineStringBuilder
}

[System.IO.DirectoryInfo] $currentDirectoryInfo = Get-Item -Path .
[string] $gitRepositoryRootDirectory =$currentDirectoryInfo.Parent.FullName
Write-Debug -Debug "DEBUG: GIT Repository Root Directory Path: $gitRepositoryRootDirectory"

Push-Location

try 
{
    Set-Location -Path $gitRepositoryRootDirectory

    WriteBuildStep "Clean Solution"
    dotnet clean --configuration Debug
    dotnet clean --configuration Release

    WriteBuildStep "Build Solution"
    dotnet build --configuration Release

    WriteBuildStep "Publish Generator"
    

    WriteBuildStep "Publish Easy to Use Generator"
}
finally
{
    Pop-Location
}

