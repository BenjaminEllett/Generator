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

function GetGeneratorStartMenuIconPath()
{
    [string] $commonStartMenuPath = [System.Environment]::GetFolderPath("CommonPrograms")
    [string] $startMenuShortCutPath = Join-Path -Path $commonStartMenuPath -ChildPath $easyToUseGeneratorShortCutFileName
    return $startMenuShortCutPath
}

function IsDirectoryInPath([string] $path, [string] $directoryPath)
{
    [string[]] $existingPathComponents = $path.Split($WindowsPathSeparator)
    foreach ($currentPathComponent in $existingPathComponents)
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