#
# This program must be run from the \PackagingAndInstall directory
#

. .\CommonPackagingFunctions.ps1

function CreateInstallProgramZipFile()
{
    VerifyScriptIsBeingRunFromTheDirectoryItIsIn
    
    [string] $publishedZipFileDirectory = GetPublishedZipFileDirectory
    [string] $packagingAndInstallDirectory = $PSScriptRoot
    Push-Location $packagingAndInstallDirectory

    try 
    {
        [string] $zipFileNameWithoutExtension = 'GeneratorInstallScripts'
        [string] $zipFileName = "$zipFileNameWithoutExtension.zip"
        [string] $installFilesZipFilePath = Join-Path -Path $publishedZipFileDirectory -ChildPath $zipFileName

        [string[]] $installScriptFiles = 'AddGeneratorToUsersPath.ps1', 'CommonInstallFunctions.ps1', 'InstallGenerator.ps1', 'UninstallGenerator.ps1'
        Compress-Archive -CompressionLevel Optimal -Path $installScriptFiles -DestinationPath $installFilesZipFilePath -ErrorAction Stop 

        [hashtable] $fileHashes = @{}
        AddFileHashToHashTable $fileHashes $zipFileName $installFilesZipFilePath
        CreateFileHashesFile $zipFileNameWithoutExtension $publishedZipFileDirectory $fileHashes
    }
    finally 
    {
        Pop-Location
    }
}

CreateInstallProgramZipFile
