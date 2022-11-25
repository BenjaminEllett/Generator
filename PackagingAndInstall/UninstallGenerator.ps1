Set-StrictMode -Version 3.0

. .\CommonFunctions.ps1

function RemoveDirectoryIfItExists([string] $directoryPath)
{
    if (Test-Path -Path $directoryPath -PathType Container)
    {
        [void](Remove-Item -Path $directoryPath -Recurse -Force -ErrorAction Stop)
    }
}

function RemoveFileIfItExists([string] $filePath)
{
    if (Test-Path -Path $filePath -PathType Leaf)
    {
        [void](Remove-Item -Path $filePath -Force -ErrorAction Stop)
    }
}



# Delete the program
[string] $generatorRootPath = GetGeneratorProgramRootPath
RemoveDirectoryIfItExists $generatorRootPath

# Remove Generator's Start Menu Shortcut
[string] $startMenuShortcutPath = GetGeneratorStartMenuShortcutPath
RemoveFileIfItExists $startMenuShortcutPath

# Remove the command line version of Generator from the user's path
[string] $userPath = GetCurrentUsersPath
[string] $commandLineGeneratorDirectoryPath = GetCommandLineGeneratorDirectoryPath

if (IsDirectoryInPath $userPath $commandLineGeneratorDirectoryPath)
{
    [string[]] $userPathComponents = $userPath.Split($WindowsPathSeparator)
    [System.Collections.Generic.List[string]] $newPathList = New-Object -TypeName 'System.Collections.Generic.List[string]'

    foreach ($currentPathComponent in $userPathComponents)
    {
        $currentPathComponent = $currentPathComponent.Normalize()
        $currentPathComponent = $currentPathComponent.Trim()

        if (-Not ($currentPathComponent -eq $commandLineGeneratorDirectoryPath))
        {
            $newPathList.Add($currentPathComponent)
        }
    }

    [string] $newPath = $newPathList -join $WindowsPathSeparator
    SetNewUserPath $newPath
}
