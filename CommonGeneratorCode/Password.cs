//
// MIT License
//
// Copyright(c) 2019-2021 Benjamin Ellett
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace CommonGeneratorCode
{
    public enum PasswordType
    {
        // Usually only used for Personal Identification numbers (PINs) on phones and smart cards.  These are very weak unless an attacker is only
        // given a limited number times (usually 10 or less) to guess the PIN.
        Numeric, 
        AlphaNumeric,
        AnyKeyOnAnEnglishKeyboardExceptASpace,
        AnyKeyOnAnEnglishKeyboard,
    };

    public enum PasswordStrength
    {
        /// <summary>
        /// This type of password can be easily guessed if an attacker is allowed an unlimited number of chances to guess it.  Passwords with 
        /// this designation can be used as PINs for phones, computers, ATM cards, etc.  The cannot be used for remote authentication because 
        /// an attacker will quickly guess the password.
        /// </summary>
        Weak,

        /// <summary>
        /// This type of password is can be used for online accounts (web sites, Windows domain accounts, etc.).  These passwords are secure 
        /// against online guessing attempts if the remote computer slows down login attemps when an attacker submits too many invalid 
        /// passwords.  These passwords are not secure against offline attacks if an attacker is willing to spend a significant amount of time 
        /// and money trying to guess a password.  
        /// 
        /// The main advantage of this type of password is it's easier to remember and many users will not use longer passwords.
        /// 
        /// In an online attack, the attacker repeatedly attempts to logon on to a remote system (i.e. web site, windows domain, etc.).  Each 
        /// attempt uses a different password.  These attacks are slower because each attempt takes more time and uses more resources 
        /// (ports, memory, CPU cycles, threads, etc.).  They can also be slower because the remote system may limit the number of attempts 
        /// which can be made.
        /// 
        /// An offline attack occurs when an attacker steals a remote system's password database.  The main advantage of an offline attack 
        /// is each password guess is much faster and the number of guesses per second cannot be limited.
        /// </summary>
        Acceptable,

        /// <summary>
        /// It is infeasible for an attacker to guess this type of password because there are too many combinations to try.  
        /// </summary>
        Strong,
    };

    public class Password
    {
        private static readonly IReadOnlyList<char> NumericCharacters = new List<char>()
                {
                    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
                };

        private static readonly IReadOnlyList<char> AlphaNumericCharacters = new List<char>(NumericCharacters)
                {
                    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                    'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',

                    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                    'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
                };

        private static readonly IReadOnlyList<char> AnyCharacterWhichCanBeTypedOnAEnglishKeyboardExceptForASpace = new List<char>(AlphaNumericCharacters)
                {
                    ')', '!', '@', '#', '$', '%', '^', '&', '*', '(',


                    '`', '-', '=', '[', ']', '\\', ';', '\'', ',', '.', '/',
                    '~', '_', '+', '{', '}', '|',  ':', '"',  '<', '>', '?',
                };

        // These characters can be typed on a standard 101 key US English keyboard.
        private static readonly IReadOnlyList<char> AnyCharacterWhichCanBeTypedOnAEnglishKeyboard = new List<char>(AnyCharacterWhichCanBeTypedOnAEnglishKeyboardExceptForASpace)
                {
                    ' '
                };

        public Password(PasswordType passwordType, int passwordLengthInCharacters)
        {
            if (passwordLengthInCharacters < Constants.MinimumPasswordLengthInChars)
            {
                throw new ArgumentException(CommonErrorMessages.PasswordLengthTooShort);
            }

            if (passwordLengthInCharacters > Constants.MaximumPasswordLengthInChars)
            {
                throw new ArgumentException(CommonErrorMessages.PasswordLengthTooLong);
            }
            
            IReadOnlyList<char> charactersWhichCanBeInPassword = GetCharactersWhichCanBeInPasswordList(passwordType);
            GeneratePassword(charactersWhichCanBeInPassword, passwordLengthInCharacters);
            DeterminePasswordStrength(charactersWhichCanBeInPassword.Count, passwordLengthInCharacters);
        }

        public string Value { get; private set; }

        public PasswordStrength Strength { get; private set; }

        public string StrengthDescription
        {
            get
            {
                switch (this.Strength)
                {
                    case PasswordStrength.Strong:
                        return CommonUserInterface.StrongPasswordDescription;

                    case PasswordStrength.Acceptable:
                        return CommonUserInterface.AcceptablePasswordDescription;

                    case PasswordStrength.Weak:
                        return CommonUserInterface.WeakPasswordDescription;

                    default:
                        throw new Exception(CommonErrorMessages.ThisCaseShouldNeverOccur);
                }
            }
        }

        public double StrengthInBits { get; private set; }

        private static IReadOnlyList<char> GetCharactersWhichCanBeInPasswordList(PasswordType passwordType)
        {
            // LOCALIZATION: This code will have to be modified if this program is modified to work with other languages.
            // Right now, it only supports generating passwords which can be typed on US English keyboards.

            switch (passwordType)
            {
                case PasswordType.Numeric:
                    return NumericCharacters;

                case PasswordType.AlphaNumeric:
                    return AlphaNumericCharacters;

                case PasswordType.AnyKeyOnAnEnglishKeyboardExceptASpace:
                    return AnyCharacterWhichCanBeTypedOnAEnglishKeyboardExceptForASpace;

                case PasswordType.AnyKeyOnAnEnglishKeyboard:
                    return AnyCharacterWhichCanBeTypedOnAEnglishKeyboard;

                default:
                    throw new Exception(CommonErrorMessages.ThisCaseShouldNeverOccur);
            }
        }

        private void GeneratePassword(IReadOnlyList<char> validPasswordCharactersList, int passwordLengthInCharacters)
        { 
#if DEBUG
            {
                const int MaximumSupportedListLength = 256;

                Debug.Assert(validPasswordCharactersList.Count <= MaximumSupportedListLength,
                             "arrayOfValidPasswordCharacters should contain no more than 256 entries.  If it contains more entries, this function will not work.");
            }
#endif

            // Create a cryptographically secure random number generator.
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            using (randomNumberGenerator)
            {
                const uint OneCharacter = 1;

                byte passwordCharacterIndex;
                byte[] passwordCharacterIndexArray = new byte[OneCharacter];

                StringBuilder newPassword = new StringBuilder(passwordLengthInCharacters);

                while (newPassword.Length < passwordLengthInCharacters)
                {
                    randomNumberGenerator.GetBytes(passwordCharacterIndexArray);
                    passwordCharacterIndex = passwordCharacterIndexArray[0];

                    if (IsThereAnEqualChanceEachCharacterCouldBeSelected(passwordCharacterIndex))
                    {
                        passwordCharacterIndex = checked((byte)(passwordCharacterIndex % ((byte)validPasswordCharactersList.Count)));
                        newPassword = newPassword.Append(validPasswordCharactersList[passwordCharacterIndex]);
                    }
                }

                this.Value = newPassword.ToString();
                return;
            }



            bool IsThereAnEqualChanceEachCharacterCouldBeSelected(byte indexOfCharacterToSelect)
            {
                const int TotalNumberOfDistinctByteValues = 256;

                int totalNumberOfWholeRangesInAByte = checked(TotalNumberOfDistinctByteValues / validPasswordCharactersList.Count);
                int indexOfLastNumberInLastWholeRange = checked((totalNumberOfWholeRangesInAByte * validPasswordCharactersList.Count) - 1);

                return (indexOfCharacterToSelect <= indexOfLastNumberInLastWholeRange);
            }
        }

        private void DeterminePasswordStrength(int numberOfPossibleCharactersPerPasswordCharacter, int passwordLengthInCharacters)
        {
            // SECURITY: The minimum length of a secure password is 8 characters.  8 characters was chosen as the minimum length based on
            // recommendations from Microsoft and the Nation Institute of Standards and Technology (NIST).  Here is NIST's
            // password guidence: 
            //
            // "5.1.1 Memorized Secrets
            //
            // A Memorized Secret authenticator - commonly referred to as a password or, if numeric, a PIN - is a secret value intended to
            // be chosen and memorized by the user. Memorized secrets need to be of sufficient complexity and secrecy that it would be
            // impractical for an attacker to guess or otherwise discover the correct secret value.  A memorized secret is something you
            // know.
            //
            // 5.1.1.1 Memorized Secret Authenticators
            //
            // Memorized secrets SHALL be at least 8 characters in length if chosen by the subscriber.  Memorized secrets chosen randomly
            // by the CSP or verifier SHALL be at least 6 characters in length and MAY be entirely numeric. If the CSP or verifier
            // disallows a chosen memorized secret based on its appearance on a blacklist of compromised values, the subscriber SHALL be
            // required to choose a different memorized secret. No other complexity requirements for memorized secrets SHOULD be imposed.
            // A rationale for this is presented in Appendix A Strength of Memorized Secrets."
            // (NIST Special Publication 800-63B - Digital Identity Guidelines - Authentication and Lifecycle Management, Document 
            // accessed on 7/5/2021, https://nvlpubs.nist.gov/nistpubs/SpecialPublications/NIST.SP.800-63b.pdf)
            //
            // Here is Microsoft's Active Directory Team's password guidence:
            //
            // "Maintain an 8-character minimum length requirement"
            // (accessed on 7/5/2021,
            // https://docs.microsoft.com/en-us/microsoft-365/admin/misc/password-policy-recommendations?view=o365-worldwide)
            // 
            const double MinimumPasswordLengthInChars = 8;

            double bitsPerPasswordCharacter = Math.Log(numberOfPossibleCharactersPerPasswordCharacter, newBase: 2);
            double passwordStrengthInBits = bitsPerPasswordCharacter * passwordLengthInCharacters;

            // SECURITY: I choose 128 bits as the minimum number of bits of entropy for a very secure password because it's infeasible to 
            //           guess all combinations of a password with 128 bits of entropy.  Even a very sophisticated attacker 
            //           cannot do this.
            const double MiniumumNumberOfBitsInAPasswordWhichIsAdequateForProtectingAllOfYourData = 128;

            PasswordStrength passwordStrength;

            if (MiniumumNumberOfBitsInAPasswordWhichIsAdequateForProtectingAllOfYourData <= passwordStrengthInBits)
            {
                passwordStrength = PasswordStrength.Strong;
            }
            else if ((MinimumPasswordLengthInChars <= passwordLengthInCharacters) &&
                     (numberOfPossibleCharactersPerPasswordCharacter >= AlphaNumericCharacters.Count))
            {
                passwordStrength = PasswordStrength.Acceptable;
            }
            else
            {
                passwordStrength = PasswordStrength.Weak;
            }

            this.Strength = passwordStrength;
            this.StrengthInBits = passwordStrengthInBits;
        }
    }
}