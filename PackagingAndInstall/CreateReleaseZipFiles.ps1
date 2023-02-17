#
# This program must be run from the \PackagingAndInstall directory
#

Set-StrictMode -Version 3.0

. .\CommonPackagingFunctions.ps1

function WriteHeader([string] $headerText)
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
    Write-Host -ForegroundColor Yellow -Object "    $headerText"
    Write-Host -ForegroundColor Yellow -Object $separatorLineStringBuilder
}

function PublishProjectAndCreateZipFiles([string] $projectName, [string] $publishedZipFileDirectory)
{
    [string] $csprojRelativeFilePath = Join-Path . $projectName "$projectName.csproj"

    if (-Not (Test-Path -Path $csprojRelativeFilePath))
    {
        throw "ERROR: Cannot find the $projectName's csproj file.  Here  is the relative path used: $csprojRelativeFilePath"
    }

    # Note that the directory location of pubxml files cannot be changed.  For more informaiton, please see the 
    # dotnet publish command's documentation.
    [string] $publishFolderRelativeFilePath = Join-Path . $projectName "Properties\PublishProfiles\FolderProfile.pubxml" 
    if (-Not (Test-Path -Path $publishFolderRelativeFilePath))
    {
        throw "ERROR: Cannot find the $projectName's publish (pubxml) file.  Here  is the relative path used: $publishFolderRelativeFilePath"
    }

    WriteHeader "Publish $projectName"

    dotnet publish $csprojRelativeFilePath -p:PublishProfile=FolderProfile.pubxml    

    [string] $publishedFilesDirectoryPath = Join-Path . $projectName "bin\Release\publish"
    if (-Not (Test-Path $publishedFilesDirectoryPath))
    {
        throw "ERROR: The dotnet publish command did not create the publish directory.  Here is the directory's path: $publishedFilesDirectoryPath"
    }

    [string] $zipFileName = "$projectName.zip"
    [string] $zipFileWithPdbFilesName = $projectName + "WithPdbFiles.zip"
    [string] $zipFilePath = Join-Path $publishedZipFileDirectory $zipFileName
    [string] $zipFileWithPdbFilesPath = Join-Path $publishedZipFileDirectory $zipFileWithPdbFilesName

    Get-ChildItem $publishedFilesDirectoryPath -Exclude "*.pdb" | Compress-Archive -CompressionLevel Optimal -DestinationPath $zipFilePath -ErrorAction Stop
    Get-ChildItem $publishedFilesDirectoryPath | Compress-Archive -CompressionLevel Optimal -DestinationPath $zipFileWithPdbFilesPath -ErrorAction Stop

    [hashtable] $fileHashes = @{}
    AddFileHashToHashTable $fileHashes $zipFileName $zipFilePath
    AddFileHashToHashTable $fileHashes $zipFileWithPdbFilesName $zipFileWithPdbFilesPath
    
    CreateFileHashesFile $projectName $publishedZipFileDirectory $fileHashes
}

function CreateReleaseZipFiles()
{
    VerifyScriptIsBeingRunFromTheDirectoryItIsIn

    [System.IO.DirectoryInfo] $currentDirectoryInfo = Get-Item -Path .
    [string] $gitRepositoryRootDirectory = $currentDirectoryInfo.Parent.FullName
    [string] $publishedZipFileDirectory = GetPublishedZipFileDirectory
        
    if (Test-Path $publishedZipFileDirectory)
    {
        Remove-Item -Path $publishedZipFileDirectory -Recurse -Force
    }

    New-Item -Path $publishedZipFileDirectory -ItemType Directory

    Push-Location

    try 
    {
        Set-Location -Path $gitRepositoryRootDirectory

        WriteHeader "Clean Solution"

        dotnet clean --configuration Debug
        dotnet clean --configuration Release

        # This is the safest and easiest way to remove all extra files from the repository
        git clean -dfx

        PublishProjectAndCreateZipFiles "Generator" $publishedZipFileDirectory
        PublishProjectAndCreateZipFiles "EasyToUseGenerator" $publishedZipFileDirectory
    }
    finally
    {
        Pop-Location
    }
}

CreateReleaseZipFiles