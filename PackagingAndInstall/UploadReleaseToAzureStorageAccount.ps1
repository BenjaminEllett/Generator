param(
    [Parameter(
        Mandatory=$true, 
        HelpMessage='The version of Generator being uploaded to the blob storage account.  This string should be in the form of v1, v1-1, v1-2-1, etc.')]
    [string] $generatorVersion
)

. .\CommonPackagingFunctions.ps1

[string] $azureStorageAccountResourceGroup = 'GeneratorApplication'
[string] $azureStorageAccountName = 'generatordownload'

#
# Make sure the release can be uploaded
# 

[string] $publishedZipFileDirectory = GetPublishedZipFileDirectory

if (-Not (Test-Path -Path $publishedZipFileDirectory -PathType Container))
{
    throw "ERROR: Cannot upload the release because the '$publishedZipFileDirectory' directory does not exist."
}

# Make sure all of the release files exist

[System.Collections.ArrayList] $releaseFiles = @(
    'EasyToUseGenerator.zip'
    'EasyToUseGeneratorWithPdbFiles.zip'
    'Generator.zip'
    'GeneratorFileHashes.json'
    'GeneratorInstallScripts.zip'
    'GeneratorInstallScriptsFileHashes.json'
    'GeneratorWithPdbFiles.zip'
    'EasyToUseGeneratorFileHashes.json'
)

foreach ($currentReleaseFileName in $releaseFiles)
{
    [string] $filePath = Join-Path -Path $publishedZipFileDirectory -ChildPath $currentReleaseFileName
    if (-Not (Test-Path -Path $filePath -PathType Leaf))
    {
        throw "ERROR: Could not upload a release because the '$filePath' file was missing."
    }
}

#
# Upload the release to an Azure storage account
#

Connect-AzAccount

try
{
    $generatorDownloadStorageAccount = Get-AzStorageAccount -ResourceGroup $azureStorageAccountResourceGroup -Name $azureStorageAccountName -ErrorAction Stop
    New-AzStorageContainer -Context $generatorDownloadStorageAccount.Context -Name $generatorVersion -Permission Blob -ErrorAction Stop
   
    Get-ChildItem -Path $publishedZipFileDirectory | Set-AzStorageBlobContent -Context $generatorDownloadStorageAccount.Context  -Container $generatorVersion -BlobType Block -Force -ErrorAction Stop
}
finally
{
    Disconnect-AzAccount
}