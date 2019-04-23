//
// MIT License
//
// Copyright(c) 2019 Benjamin Ellett
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

using Generator.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Generator
{
    public enum PasswordType
    {
        // Usually only used for Personal Identification numbers (PINs) on phones and smart cards.  These are very weak unless an attacker is only
        // given a limited number times (usually 10 or less) to guess the PIN.
        Numeric, 
        AlphaNumericPassword,
        AnyKeyOnAnEnglishKeyboard,
    };

    public enum PasswordStrength
    {
        Weak,
        AdequateForProtectingAllOfYourData
    };

    public class Password
    {
        public Password(PasswordType passwordType, int passwordLengthInCharacters)
        {
            if (passwordLengthInCharacters < Constants.MINIMUM_PASSWORD_LENGTH_IN_CHARACTERS)
            {
                throw new ArgumentException(ErrorMessages.PasswordLengthTooShort);
            }

            if (passwordLengthInCharacters > Constants.MAXIMUM_PASSWORD_LENGTH_IN_CHARACTERS)
            {
                throw new ArgumentException(ErrorMessages.PasswordLengthTooLong);
            }
            
            IReadOnlyList<char> charactersWhichCanBeInPassword = GetCharactersWhichCanBeInPasswordList(passwordType);
            GeneratePassword(charactersWhichCanBeInPassword, passwordLengthInCharacters);
            DeterminePasswordStrength(charactersWhichCanBeInPassword.Count, passwordLengthInCharacters);
        }

        public string Value { get; private set; }

        public PasswordStrength Strength { get; private set; }

        public double StrengthInBits { get; private set; }

        private IReadOnlyList<char> GetCharactersWhichCanBeInPasswordList(PasswordType passwordType)
        {
            // LOCALIZATION: This code will have to be modified if this program is modified to work with other languages.
            // Right now, it only supports generating passwords which can be typed on English keyboards.

            List<char> NUMERIC_CHARACTERS = new List<char>()
                {
                    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
                };

            List<char> ALPHA_NUMERIC_CHARACTERS = new List<char>()
                {
                    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                    'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',

                    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                    'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
                };
            ALPHA_NUMERIC_CHARACTERS.AddRange(NUMERIC_CHARACTERS);

            // These characters can be typed on a standard 101 key US English keyboard.
            List<char> ANY_CHARACTER_WHICH_CAN_BE_TYPED_ON_A_ENGLISH_KEYBOARD = new List<char>()
                {
                    ')', '!', '@', '#', '$', '%', '^', '&', '*', '(',


                    '`', '-', '=', '[', ']', '\\', ';', '\'', ',', '.', '/',
                    '~', '_', '+', '{', '}', '|',  ':', '"',  '<', '>', '?',

                    ' '
                };
            ANY_CHARACTER_WHICH_CAN_BE_TYPED_ON_A_ENGLISH_KEYBOARD.AddRange(ALPHA_NUMERIC_CHARACTERS);

            switch (passwordType)
            {
                case PasswordType.Numeric:
                    return NUMERIC_CHARACTERS;

                case PasswordType.AlphaNumericPassword:
                    return ALPHA_NUMERIC_CHARACTERS;

                case PasswordType.AnyKeyOnAnEnglishKeyboard:
                    return ANY_CHARACTER_WHICH_CAN_BE_TYPED_ON_A_ENGLISH_KEYBOARD;

                default:
                    throw new Exception("This case should never occur.");
            }
        }

        private void GeneratePassword(IReadOnlyList<char> validPasswordCharactersList, int passwordLengthInCharacters)
        { 
#if DEBUG
            {
                const int MAXIMUM_SUPPORTED_LIST_LENGTH = 256;

                Debug.Assert(validPasswordCharactersList.Count <= MAXIMUM_SUPPORTED_LIST_LENGTH,
                             "arrayOfValidPasswordCharacters should contain no more than 256 entries.  If it contains more entries, this function will not work.");
            }
#endif

            // Create a cryptographically secure random number generator.
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            using (randomNumberGenerator)
            {
                const uint ONE_CHARACTER = 1;

                byte passwordCharacterIndex;
                byte[] passwordCharacterIndexArray = new byte[ONE_CHARACTER];

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
                const int TOTAL_NUMBER_OF_DISTINCT_BYTE_VALUES = 256;

                int totalNumberOfWholeRangesInAByte = checked(TOTAL_NUMBER_OF_DISTINCT_BYTE_VALUES / validPasswordCharactersList.Count);
                int indexOfLastNumberInLastWholeRange = checked((totalNumberOfWholeRangesInAByte * validPasswordCharactersList.Count) - 1);

                return (indexOfCharacterToSelect <= indexOfLastNumberInLastWholeRange);
            }
        }

        private void DeterminePasswordStrength(int numberOfPossibleCharactersPerPasswordCharacter, int passwordLengthInCharacters)
        {
            double bitsPerPasswordCharacter = Math.Log(numberOfPossibleCharactersPerPasswordCharacter, newBase: 2);
            double passwordStrengthInBits = bitsPerPasswordCharacter * passwordLengthInCharacters;

            // SECURITY: I choose 128 bits as the minimum number of bits of entropy for a secure password because it's infeasible to 
            //           guess all combinations of a password with 128 bits of entropy.  Even a very sophisticated attacked 
            //           cannot do this.
            const double MINIUMUM_NUMBER_OF_BITS_IN_A_PASSWORD_WHICH_IS_ADEQUATE_FOR_PROTECTING_ALL_OF_YOUR_DATA = 128;

            PasswordStrength passwordStrength;

            if (MINIUMUM_NUMBER_OF_BITS_IN_A_PASSWORD_WHICH_IS_ADEQUATE_FOR_PROTECTING_ALL_OF_YOUR_DATA <= passwordStrengthInBits)
            {
                passwordStrength = PasswordStrength.AdequateForProtectingAllOfYourData;
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