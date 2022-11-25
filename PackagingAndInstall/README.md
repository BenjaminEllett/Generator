# How to test that install/uninstall worked

Preform the following steps to verify an install or uninstall operation worked:

1. Open a **new** Windows Terminal after the install or uninstall operation is complete.

2. Open a PowerShell tab in Windows Terminal.

3. Run the following commands.  They should all return $true if Generator is installed and the command line version was added to the path.  They should all return $false if it is not installed.

        Test-Path -Path "$env:ProgramFiles\Generator" -PathType Container
        Test-Path -Path "$env:ALLUSERSPROFILE\Microsoft\Windows\Start Menu\Programs\Generator.lnk" -PathType Leaf
        ($env:PATH).Contains("$env:ProgramFiles\Generator\Generator")

4. Type **generator / AKB 20** .  If Generator's command line version is insalled and its directory was added to the PATH, it should run and display a new password.

5.  Open Window's Start Menu.

6. If Generator is installed, you should be able to find it on the **All Apps** sub-menu.

7. The Generator icon/shortcut on the **All Apps** menu should launch the easy to use (GUI) version of Generator if its installed.