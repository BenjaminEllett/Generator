# Generator

Generator is a family of programs which which use a cryptographically secure random number generator to create truly random passwords.  Here are its features:

- It can create passwords which have 1 to at least 38 characters.  The characters can come from one of the following sets:
    - Any character which can be typed on a United States English 101 key keyboard
    - Alpha-numeric characters (a-z, A-Z and 0-9)
    - Numeric characters (0-9)
    
- It displays a new password's strength 

- Anyone can easily read the code and verify it is secure.  They can do this because the program is short and the code is not complicated.

- Generator uses one of the built in .NET secure random number generator classes.  It uses System.Security.Cryptography.RandomNumberGenerator (https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.randomnumbergenerator?view=net-5.0).

Generator has two versions:

- Command line 
- Windows App

The two applications are very similar.  Here are the differences:

- The Windows App version is easier to use.  It has a graphical user interface (GUI) and is targeted at users who do not use command line applications.

- The command line version supports spaces in passwords.  The GUI version prohibits spaces in passwords in order to improve usability. 

- The command line version supports passwords from 1 to 256 characters.  The GUI version supports passwords which have 1 to 38 characters.

# App Documentation

Here are links to each program's documentation:

[Windows App Documentation](Documentation/WindowsAppGeneratorUsage.md)

[Command Line App Documentation](Documentation/CommandLineAppUsage.md)

# Links

[Password Security Advice](Documentation/PasswordSecurityAdvice.md)

This page explains how to use passwords correctly.  Using passwords correctly minimizes the chance that your accounts get broken into.

https://nvlpubs.nist.gov/nistpubs/SpecialPublications/NIST.SP.800-63b.pdf

The National Institutes of Standards and Technology ([NIST](https://www.nist.gov/)) is a United States Government Agency which creates standards.  Section 5.1.1.1 of this document recommends that passwords have at least 8 characters.   

https://github.com/BenjaminEllett/Generator

This is a link to Generator’s web site.  The site contains the program’s code, automated tests, issues (bugs and work items) and project status.

https://github.com/BenjaminEllett/Generator/releases
     
This is a link to Generator's releases.  You can go her to find the latest version of Generator and you download it from here.

https://github.com/BenjaminEllett/Generator/issues

This is a link to Generator’s work items and bugs. You can use this page to request new features or file bug reports.

https://github.com/BenjaminEllett/Generator/projects/1 

This page has the status of the current release.  You can see what is done and what is left to do.

https://github.com/BenjaminEllett/Generator/blob/main/CommonGeneratorCode/Password.cs

This is a link to the C# class which creates the random passwords.  It does all of the important work.
