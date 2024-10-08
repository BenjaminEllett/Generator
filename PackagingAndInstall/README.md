# How to make a Generator Release

1. Update the version number in the .NET applications

2. Run the following PowerShell commands from the root of Generator's GIT repository

        Set-Location .\PackagingAndInstall
        .\CreateReleaseZipFiles.ps1

3. Update the URIs and SHA512 hashes in the InstallGenerator.ps1 script.  You can find the hashes in $HOME\Documents\PublishedZipFiles\EasyToUseGeneratorFileHashes.json and $HOME\Documents\PublishedZipFiles\GeneratorFileHashes.json.  You can get the hashes by running the following PowerShell commands:

        Get-Content $HOME\Documents\PublishedZipFiles\EasyToUseGeneratorFileHashes.json
        Get-Content $HOME\Documents\PublishedZipFiles\GeneratorFileHashes.json

4. Run the following PowerShell commands.  PowerShell should still be in the PackagingAndInstall directory.

        .\CreateInstallProgramZipFile.ps1

5. Update the SHA512 hashes and URIs in this file.  The hashes are in the **Instructions for downloading and installing the program** section.  You can get the new hash value by running the following PowerShell command:

        Get-Content $HOME\Documents\PublishedZipFiles\GeneratorInstallScriptsFileHashes.json

6. Run the following PowerShell commands to upload the release binaries to Generator's Azure storage account:

        [int] $majorVersion = <REPLACE_WITH_MAJOR_VERSION_NUMBER>
        [int] $minorVersion = <REPLACE_WITH_MINOR_VERSION_NUMBER>
        .\UploadReleaseToAzureStorageAccount.ps1 "v$majorVersion-$minorVersion"

7. Make sure all changes are checked in

8. Go to GitHub and create the release.  This includes creating the GIT tag with the version.

9. Test install again to make sure it works in production.



# Instructions for downloading and installing the program

Here is how you install Generator:

1. Install the .NET 8.0 desktop runtime if you do not have it on your machine. Here is a link to the runtime: https://dotnet.microsoft.com/en-us/download/dotnet/8.0 or https://dotnet.microsoft.com/download .  You can also install it by running the following winget command:

        winget install --id Microsoft.DotNet.DesktopRuntime.8 --source winget

2. Run PowerShell as an Administrator user

3. Run **one** of the following PowerShell commands.  Each command is one long line.  The first command installs the easy to use version of Generator (i.e. the GUI version).  The second command installs the easy to use version and the command line version.

        Invoke-WebRequest -Method GET -SslProtocol Tls12 -Uri 'https://generatordownload.blob.core.windows.net/v1-4/GeneratorInstallScripts.zip' -OutFile .\GeneratorInstallScripts.zip ; if ('A6B723FED301B0A7FD00A8DA937A052468A1FBF0C9E61B313A441040F02369DB2B2F47062C10C371404D4C0B9E9D68250F6E74B8DE894B5B65D6583F34F3E729' -ne (Get-FileHash -Algorithm SHA512 -Path .\GeneratorInstallScripts.zip).Hash) { throw "ERROR: Downloaded installation program is corrupted." }; Expand-Archive -Path .\GeneratorInstallScripts.zip -DestinationPath .\GeneratorInstallScripts; Remove-Item -Path .\GeneratorInstallScripts.zip; Set-Location .\GeneratorInstallScripts; .\InstallGenerator.ps1

        Invoke-WebRequest -Method GET -SslProtocol Tls12 -Uri 'https://generatordownload.blob.core.windows.net/v1-4/GeneratorInstallScripts.zip' -OutFile .\GeneratorInstallScripts.zip ; if ('A6B723FED301B0A7FD00A8DA937A052468A1FBF0C9E61B313A441040F02369DB2B2F47062C10C371404D4C0B9E9D68250F6E74B8DE894B5B65D6583F34F3E729' -ne (Get-FileHash -Algorithm SHA512 -Path .\GeneratorInstallScripts.zip).Hash) { throw "ERROR: Downloaded installation program is corrupted." }; Expand-Archive -Path .\GeneratorInstallScripts.zip -DestinationPath .\GeneratorInstallScripts; Remove-Item -Path .\GeneratorInstallScripts.zip; Set-Location .\GeneratorInstallScripts; .\InstallGenerator.ps1 -InstallCommandLineVersion

4. If you installed the command line version, you can run .\AddGeneratorToUsersPath.ps1 to add Generator to your path.

5. Go to the parent directory

6. Remove the scripts folder.  The easiest way to do this is to run the following PowerShell commands:

        Remove-Item -Path .\GeneratorInstallScripts -Recurse -Force

Now that the program is installed, you can find the GUI version on the Start Menu (it's called Generator).  The command line version is in $env:ProgramFiles\Generator\Generator (usually C:\Program Files\Generator\Generator).  If you added Generator to your path, you will need to open a new terminal window (not a tab) or command prompt window to see the path change.



# Instructions for locally testing the one line command to download and install Generator

Take each string and replace the Invoke-Webrequest command with the following PowerShell command:

        Copy-Item -Path "$HOME\Documents\PublishedZipFiles\GeneratorInstallScripts.zip" -Destination .



# How to test that install/uninstall works

Preform the following steps to verify an install or uninstall operation worked:

1. Open a **new** Windows Terminal after the install or uninstall operation is complete.

2. Open a PowerShell tab in Windows Terminal.

3. Run the following commands.  They should all return $true if Generator is installed and the command line version was added to the path.  They should all return $false if it is not installed.

        Test-Path -Path "$env:ProgramFiles\Generator" -PathType Container
        Test-Path -Path "$env:ALLUSERSPROFILE\Microsoft\Windows\Start Menu\Programs\Generator.lnk" -PathType Leaf
        ($env:PATH).Contains("$env:ProgramFiles\Generator\Generator")

4. Type **generator / AKB 20** .  If Generator's command line version is insalled and its directory was added to the PATH, it should run and display a new password.

5. Open Window's Start Menu.

6. If Generator is installed, you should be able to find it on the **All Apps** sub-menu.

7. The Generator icon/shortcut on the **All Apps** menu should launch the easy to use (GUI) version of Generator if its installed.



# Useful PowerShell commands for testing if Generator was uploaded to Azure

        $a = Get-AzStorageAccount -ResourceGroup 'GeneratorApplication' -Name 'generatordownload'
        Get-AzStorageContainer -Context $a.Context
