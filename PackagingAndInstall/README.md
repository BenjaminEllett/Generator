# Check list for making a Generator release

1. Update the version number in the .NET applications

2. Update the SHA512 hashes and URIs in the following documentation (Instructions for downloading and installing the program)

3. Update the URIs and SHA512 hashes in the InstallGenerator.ps1 script



# Instructions for downloading and installing the program

Here is how you install Generator:

1. Open Windows Terminal as an Administrator user

2. Start a PowerShell 7 tab

3. Run **one** of the PowerShell commands.  Each command is one long line.  The first command installs the easy to use version of Generator (i.e. the GUI version).  The second command installs the easy to use version and the command line version.

        Invoke-WebRequest -Method GET -SslProtocol Tls12 -Uri 'https://generatordownload.blob.core.windows.net/v1-3/GeneratorInstallScripts.zip' -OutFile .\GeneratorInstallScripts.zip ; if ('7E63336160A1C8837CCCAB25440E906CD65774F1FE8E95F2A0EF62E4B037B957086FD53D94BE63BF8D91BA9C54CF0457721B54C6180584E377D3B52484B9B755' -ne (Get-FileHash -Algorithm SHA512 -Path .\GeneratorInstallScripts.zip).Hash) { throw "ERROR: Downloaded installation program is corrupted." }; Expand-Archive -Path .\GeneratorInstallScripts.zip -DestinationPath .\GeneratorInstallScripts; Remove-Item -Path .\GeneratorInstallScripts.zip; Set-Location .\GeneratorInstallScripts; .\InstallGenerator.ps1

        Invoke-WebRequest -Method GET -SslProtocol Tls12 -Uri 'https://generatordownload.blob.core.windows.net/v1-3/GeneratorInstallScripts.zip' -OutFile .\GeneratorInstallScripts.zip ; if ('7E63336160A1C8837CCCAB25440E906CD65774F1FE8E95F2A0EF62E4B037B957086FD53D94BE63BF8D91BA9C54CF0457721B54C6180584E377D3B52484B9B755' -ne (Get-FileHash -Algorithm SHA512 -Path .\GeneratorInstallScripts.zip).Hash) { throw "ERROR: Downloaded installation program is corrupted." }; Expand-Archive -Path .\GeneratorInstallScripts.zip -DestinationPath .\GeneratorInstallScripts; Remove-Item -Path .\GeneratorInstallScripts.zip; Set-Location .\GeneratorInstallScripts; .\InstallGenerator.ps1 -InstallCommandLineVersion

4. If you installed the command line version, you can run .\AddGeneratorToUsersPath.ps1 to add Generator to your path.

5. Go to the parent directory

6. Remove the scripts folder.  The easiest way to do this is to run the following PowerShell commands:

        Remove-Item -Path .\GeneratorInstallScripts -Recurse -Force

Now that the program is installed, you can find the GUI version on the Start Menu (it's called Generator).  The command line version is in $env:ProgramFiles\Generator\Generator (usually C:\Program Files\Generator\Generator).  If you added Generator to your path, you will need to open a new terminal window (not tab) or command prompt window to see the path change.

# Instructions for locally testing the one line command to download and install Generator

Take each string and replace the Invoke-Webrequest command with the following PowerShell command:

        Copy-Item -Path "$HOME\Documents\PublishedZipFiles\GeneratorInstallScripts.zip" -Destination .

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

