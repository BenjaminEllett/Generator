Set-StrictMode -Version 3.0

function GetPublishedZipFileDirectory()
{
    [string] $documentsFolder = [System.Environment]::GetFolderPath("MyDocuments")
    [string] $publishedZipFileDirectory = Join-Path $documentsFolder "PublishedZipFiles"
    return $publishedZipFileDirectory
}

function AddFileHashToHashTable([hashtable] $ht, [string] $fileName, [string] $filePath)
{
    $getHashResult = Get-FileHash -Algorithm SHA512 -Path $filePath
    $ht.Add($fileName, $getHashResult.Hash)
}

function CreateFileHashesFile([string] $fileHashesFilePrefix, [string] $publishedZipFileDirectory, [hashtable] $fileHashes)
{
    [string] $fileHashesFileName = $fileHashesFilePrefix + "FileHashes.json"
    [string] $fileHashesFilePath = Join-Path $publishedZipFileDirectory $fileHashesFileName
    ConvertTo-Json $fileHashes | Out-File -FilePath $fileHashesFilePath
}

function VerifyScriptIsBeingRunFromTheDirectoryItIsIn()
{
    if ($PSScriptRoot -ne $PWD) 
    {
        throw "ERROR: This program can only be run from the '$PSScriptRoot' directory."
    }
}