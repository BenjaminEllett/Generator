# How to test that install worked

Run the following PowerShell commands in a new PowerShell window (not a new tab):

    Test-Path -Path "$env:ProgramFiles\Generator" -PathType Container
    Test-Path -Path "$env:ALLUSERSPROFILE\Microsoft\Windows\Start Menu\Programs\Generator.lnk" -PathType Leaf
    ($env:PATH).Contains("$env:ProgramFiles\Generator\Generator")

All commands should return $true .

# How to test if Generator is uninstalled

Run the following PowerShell commands in a new PowerShell window (not a new tab):

    Test-Path -Path "$env:ProgramFiles\Generator" -PathType Container
    Test-Path -Path "$env:ALLUSERSPROFILE\Microsoft\Windows\Start Menu\Programs\Generator.lnk" -PathType Leaf
    ($env:PATH).Contains("$env:ProgramFiles\Generator\Generator")

All commands should return $false .
