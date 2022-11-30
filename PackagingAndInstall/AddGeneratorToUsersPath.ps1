Set-StrictMode -Version 3.0

. .\CommonInstallFunctions.ps1

[string] $currentUserPath = GetCurrentUsersPath
[string] $commandLineGeneratorDirectoryPath = GetCommandLineGeneratorDirectoryPath

if (IsDirectoryInPath $currentUserPath $commandLineGeneratorDirectoryPath)
{
    return;
}

[string] $valueToAdd = $null 

if ($currentUserPath.EndsWith($WindowsPathSeparator)) 
{ 
    $valueToAdd = $commandLineGeneratorDirectoryPath
} 
else 
{ 
    $valueToAdd = $WindowsPathSeparator + $commandLineGeneratorDirectoryPath
}

$newUserPath = $currentUserPath + $valueToAdd
    
SetNewUserPath $newUserPath
