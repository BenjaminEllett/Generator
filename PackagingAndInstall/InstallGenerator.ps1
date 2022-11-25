param(
    [Parameter(HelpMessage="Install Generator's command line version.  By default, only the GUI version of the application is installed.")]
    [switch] $InstallCommandLineVersion
)

#
# LIMITATIONS: This install script only works on Windows and only supports the English language.
#

Set-StrictMode -Version 3.0

. .\CommonFunctions.ps1

#
# Constants
#

[string] $easyToUseGeneratorDownloadUri = 'https://generatordownload.blob.core.windows.net/v1-2-1/EasyToUseGenerator.zip'
[string] $easyToUseGeneratorExpectedFileHash = '226FD3608E56BB11DB90694148207EE5BD6C41F34F735E0CC209EF570DA44E6971A95EF362272B2D8A93DD3E6A270CA3FBCCBCEB4949B0356E8D9863FAA45B55'

[string] $commandLineGeneratorDownloadUri = 'https://generatordownload.blob.core.windows.net/v1-2-1/Generator.zip'
[string] $commandLineGeneratorExpectedFileHash = 'E81678DCF1C11FE158D15FE9599EFF00F4C1A0E36AADD3B06964D378723FD644C1B149DB29E810373B17C3D63108D4B2BF2808539DF2019F23313683FDB1E8F1'

[string] $addGeneratorToUsersPathUri = 'https://generatordownload.blob.core.windows.net/v1-2-1/AddGeneratorToUsersPath.ps1'

function WriteBlankLine()
{
    Write-Host ""
}

function CreateDirectory([string] $newDirectoryPath)
{
    [void](New-Item -Type Directory -Path $newDirectoryPath -ErrorAction Stop)
}

function InstallProgram([string] $programDirectory, [string] $programUri, [string] $downloadedZipFileNamePrefix, [string] $expectedDownloadHash)
{
    CreateDirectory $programDirectory

    Push-Location
    
    [bool] $createdZipFile = $false

    try
    {
        Set-Location $programDirectory
    
        [System.DateTime] $now = [System.DateTime]::UtcNow

        # For more information on the date/time formate specifier passed to ToString(), please see 
        # https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings .
        [string] $zipFileName = $downloadedZipFileNamePrefix + ' ' + $now.ToString('yyyy-MM-dd HH.mm.ss.fffffff') + '.zip'
        [string] $tempDirectoryPath = [System.IO.Path]::GetTempPath()
        [string] $downloadedZipFilePath = Join-Path -Path $tempDirectoryPath -ChildPath $zipFileName

        # Invoke-WebRequest throws an exception if an error occurs
        Invoke-WebRequest -Method GET -SslProtocol Tls12 -Uri $programUri -OutFile $downloadedZipFilePath
        if (-Not (Test-Path -Path $downloadedZipFilePath))
        {
            throw "ERROR: Could not download '$downloadedZipFilePath'"
        }

        $createdZipFile = $true
        
        $fileHashResult = Get-FileHash -Algorithm SHA512 $downloadedZipFilePath
        if ($fileHashResult.Hash -ne $expectedDownloadHash)
        {
            throw "ERROR: The downloaded file is either corrupted or contains malware."
        }
    
        Expand-Archive -Path $downloadedZipFilePath -DestinationPath . -ErrorAction Stop
    }
    finally
    {
        if ($createdZipFile)
        {
            Remove-Item -Path $downloadedZipFilePath -ErrorAction Stop
        }

        Pop-Location
    }    
}

function CreateEasyToUseGeneratorShortcut([string] $easyToUseGeneratorPath)
{
    # Here is some documentation on how to use WScript.Shell to create a shortcut:
    # https://learn.microsoft.com/en-us/troubleshoot/windows-client/admin-development/create-desktop-shortcut-with-wsh
    # https://learn.microsoft.com/en-us/previous-versions//xsy6k3ys(v=vs.85)
    
    [string] $startMenuShortcutPath = GetGeneratorStartMenuShortcutPath
    [string] $executablePath = Join-Path -Path $easyToUseGeneratorPath -ChildPath 'EasyToUseGenerator.exe' 

    $wsciptShellObject = New-Object -ComObject 'Wscript.shell'

    $shortcutShellLink = $wsciptShellObject.CreateShortcut($startMenuShortcutPath)
    $shortcutShellLink.TargetPath = $executablePath
    $shortcutShellLink.WorkingDirectory = $easyToUseGeneratorPath
    $shortcutShellLink.Description = 'The program creates truly random passwords.'
    $shortcutShellLink.Save()
}

function InstallGenerator()
{
    [bool] $createdGeneratorRootDirectory = $false
    [string] $generatorRootPath = GetGeneratorProgramRootPath
    
    #
    # Remove the Generator existing directory if it exists
    #

    . .\UninstallGenerator.ps1

    CreateDirectory $generatorRootPath
    $createdGeneratorRootDirectory = $true

    #
    # Install the program
    # 

    [string] $easyToUseGeneratorPath = Join-Path -Path $generatorRootPath -ChildPath "Easy to Use Generator"
    InstallProgram $easyToUseGeneratorPath $easyToUseGeneratorDownloadUri 'EasyToUserGenerator' $easyToUseGeneratorExpectedFileHash 

    if ($InstallCommandLineVersion)
    {
        [string] $commandLineGeneratorPath = GetCommandLineGeneratorDirectoryPath
        InstallProgram $commandLineGeneratorPath $commandLineGeneratorDownloadUri 'CommandLineGenerator' $commandLineGeneratorExpectedFileHash
    }

    #
    # Create Generator Shortcut
    #

    CreateEasyToUseGeneratorShortcut $easyToUseGeneratorPath 

    #
    # Tell PowerShell users how to add Generator to the path if they want to
    #
    if ($InstallCommandLineVersion)
    {
        WriteBlankLine
        Write-Host "You can add the command line version of Generator to your PATH by running the following PowerShell commands:"
        WriteBlankLine
        Write-Host "    Invoke-WebRequest -Method GET -SslProtocol Tls12 -Uri $addGeneratorToUsersPathUri -OutFile '.\AddGeneratorToUsersPath.ps1' "
        Write-Host "    AddGeneratorToUsersPath.ps1 "
        Write-Host "    Remote-Item -Path .\AddGeneratorToUsersPath.ps1 "
        WriteBlankLine
    }
    
    return

    trap
    {
        if ($createdGeneratorRootDirectory)
        {
            . .\UninstallGenerator.ps1
        }

        break;
    }
}

InstallGenerator
