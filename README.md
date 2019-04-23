# Generator
Generator is a command line tool which uses a cryptographically secure random number generator to create truly random passwords.  Here are the program's features:

- It can create passwords which have 1 to 256 characters.  The characters can come from one of the following sets:
    - Alpha-numeric characters (a-z, A-Z and 0-9)
    - Any character which can be typed on a United States English 101 key keyboard
- It displays the password's strength in bits.
- Anyone can easily read the code and verify it is secure.  They can do this because they program is short and the code is not complicated.
- Generator uses one of .NET Core's built in secure random number generator classes.  It uses System.Security.Cryptography.RandomNumberGenerator (https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.randomnumbergenerator?view=netcore-2.1).


# Generator Usage Documentation 

```
Generator [/AKB | /AN] [new password length]
Generator /GenerateAlphaNumericPassword [new password length]
Generator /GeneratePasswordUsingAnyCharacterOnAKeyboard] [new password length]
Generator /?
Generator /Help

 - /AN
 - /GenerateAlphaNumericPassword
     If this command is specified, a new password is generated using only
     letters and numbers.  

 - /AKB
 - /GeneratePasswordUsingAnyCharacterOnAKeyboard
     If this command is specified, the created password can have any character
     which can be typed on a keyboard.  Each character in the password can be a  
     letter, number, punctuation character (“ . , ‘), mathematical operator (+ - 
     / *), bracket ([ ] { }), parenthesis ( ( ) ), etc.

- /?
- /Help
     This message is displayed.

- All commands are case insensitive.  /AKB, /? and /GenerateAlphaNumericPassword
  are examples of commands.

- All password lengths are in characters.

- Only one command can be specified.  

- Generator only supports US English Keyboards

Examples:

Generator /AKB 20
- Creates a password using any character a user can type.  The password is 20
characters long.

Generator /AN 32
- Creates a password using only letters and numbers.  The password is 32
characters long.

Generator /?
- This message is displayed.
 ```