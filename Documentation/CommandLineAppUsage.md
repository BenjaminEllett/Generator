# Generator Command Line Version Usgae Documentation

```
Generator [/AKB | /AN | /PIN] [new password length]
Generator /GeneratePasswordUsingAnyCharacterOnAKeyboard] [new password length]
Generator /GenerateAlphaNumericPassword [new password length]
Generator /GeneratePersonalIdentificationNumber [new password length]
Generator /?
Generator /Help

/AKB
/GeneratePasswordUsingAnyCharacterOnAKeyboard
     If this command is specified, the new password can have any character
     which can be typed on a keyboard.  Each character in the password can be 
     a letter, number, punctuation character (“ . , ‘), mathematical operator
     (+ - / *), bracket ([ ] { }), parenthesis ( ( ) ), etc.

/AN
/GenerateAlphaNumericPassword
     If this command is specified, a new password is generated using only
     letters and numbers.  

/PIN
/GeneratePersonalIdentificationNumber
     If this command is specified, a new password is generated using only
     numbers.  This type of passwords is typically called a PIN.  PINs should
     only be used on devices which limit the number of attempts.  iPhone lock screens,
     Smart cards and USB security keys are examples are things which limit
     the number of PIN guesses.  PINs should never be used for web sites,
     computers or any password for a remote device.  Note that Windows has
     PINs and passwords.  On Windows, passwords should be complex (have
     the possibility of having letters, numbers and symbols) because they can
     be used for remote authentication while PINs can be short because they
     are only used for local authentication (which allows Windows to limit
     the number of PIN guesses).

/?
/Help
     This message is displayed.

- All commands are case insensitive.  /AKB, /? and
  /GenerateAlphaNumericPassword are examples of commands.

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

Generator /PIN 6
- Creates a 6 digit password using only numbers.  PINs should only be used on
devices which limit the number of attempts to guess a PIN.  iPhone lock screen PINs, smart cards and USB security keys are examples are devices which limit the number of PIN guesses.

Generator /?
- This message is displayed.
 ```