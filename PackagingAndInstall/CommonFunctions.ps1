Set-StrictMode -Version 3.0

[string] $easyToUseGeneratorShortCutFileName = 'Generator.lnk'

[string] $WindowsPathSeparator = ';'

function GetGeneratorProgramRootPath()
{
    return Join-Path -Path $env:ProgramFiles -ChildPath "Generator"
}

function GetCommandLineGeneratorDirectoryPath()
{
    [string] $generatorProgramRootPath = GetGeneratorProgramRootPath
    return Join-Path -Path $generatorProgramRootPath -ChildPath "Generator"
}

function GetGeneratorStartMenuShortcutPath()
{
    [string] $commonStartMenuPath = [System.Environment]::GetFolderPath("CommonPrograms")
    [string] $startMenuShortcutPath = Join-Path -Path $commonStartMenuPath -ChildPath $easyToUseGeneratorShortCutFileName
    return $startMenuShortcutPath
}

function IsDirectoryInPath([string] $path, [string] $directoryPath)
{
    [string[]] $pathComponents = $path.Split($WindowsPathSeparator)
    foreach ($currentPathComponent in $pathComponents)
    {
        $currentPathComponent = $currentPathComponent.Normalize()
        $currentPathComponent = $currentPathComponent.Trim()
    
        if ($currentPathComponent -eq $directoryPath)
        {
            return $true
        }
    }    

    return $false
}

function GetCurrentUsersPath()
{
    return [System.Environment]::GetEnvironmentVariable("PATH", [System.EnvironmentVariableTarget]::User)
}

function SetNewUserPath([string] $newUserPath)
{
    [System.Environment]::SetEnvironmentVariable("PATH", $newUserPath, [System.EnvironmentVariableTarget]::User)    
}