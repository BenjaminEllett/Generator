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

using CommonGeneratorCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CommonGeneratorCodeUnitTests
{
    [TestClass]
    public class PasswordUnitTests
    {
        [DataTestMethod]

        // Test the lower boundary condition - Passwords with 1 character.
        [DataRow(PasswordType.Numeric, 1, PasswordStrength.Weak, 3.0)]
        [DataRow(PasswordType.AlphaNumeric, 1, PasswordStrength.Weak, 5.0)]
        [DataRow(PasswordType.AnyKeyOnAnEnglishKeyboardExceptASpace, 1, PasswordStrength.Weak, 6.0)]
        [DataRow(PasswordType.AnyKeyOnAnEnglishKeyboard, 1, PasswordStrength.Weak, 6.0)]

        // Test the common PIN lengths
        [DataRow(PasswordType.Numeric, 4, PasswordStrength.Weak, 13.0)]
        [DataRow(PasswordType.Numeric, 6, PasswordStrength.AcceptableOnlyForPins, 19.0)]
        [DataRow(PasswordType.Numeric, 8, PasswordStrength.AcceptableOnlyForPins, 26.0)]

        // Test minimum acceptable password lengths
        [DataRow(PasswordType.AlphaNumeric, 8, PasswordStrength.Acceptable, 47.0)]
        [DataRow(PasswordType.AnyKeyOnAnEnglishKeyboardExceptASpace, 8, PasswordStrength.Acceptable, 52.0)]
        [DataRow(PasswordType.AnyKeyOnAnEnglishKeyboard, 8, PasswordStrength.Acceptable, 52.0)]

        // Test common password lengths 
        [DataRow(PasswordType.AlphaNumeric, 16, PasswordStrength.Acceptable, 95.0)]
        [DataRow(PasswordType.AlphaNumeric, 20, PasswordStrength.Acceptable, 119.0)]
        [DataRow(PasswordType.AlphaNumeric, 24, PasswordStrength.Strong, 142.0)]

        [DataRow(PasswordType.AnyKeyOnAnEnglishKeyboardExceptASpace, 16, PasswordStrength.Acceptable, 104.0)]
        [DataRow(PasswordType.AnyKeyOnAnEnglishKeyboardExceptASpace, 20, PasswordStrength.Strong, 131.0)]
        [DataRow(PasswordType.AnyKeyOnAnEnglishKeyboardExceptASpace, 24, PasswordStrength.Strong, 157.0)]

        [DataRow(PasswordType.AnyKeyOnAnEnglishKeyboard, 16, PasswordStrength.Acceptable, 105.0)] 
        [DataRow(PasswordType.AnyKeyOnAnEnglishKeyboard, 20, PasswordStrength.Strong, 131.0)]
        [DataRow(PasswordType.AnyKeyOnAnEnglishKeyboard, 24, PasswordStrength.Strong, 157.0)]

        // Test the upper boundary condition - Passwords with the maximum number of characters (256).
        [DataRow(PasswordType.Numeric, 256, PasswordStrength.Strong, 850.0)] 
        [DataRow(PasswordType.AlphaNumeric, 256, PasswordStrength.Strong, 1524.0)]
        [DataRow(PasswordType.AnyKeyOnAnEnglishKeyboardExceptASpace, 256, PasswordStrength.Strong, 1677.0)]
        [DataRow(PasswordType.AnyKeyOnAnEnglishKeyboard, 256, PasswordStrength.Strong, 1681.0)]

        /// <summary>
        /// BasicPasswordTest ensures that the Password object can all types of passwords correctly.  It also ensures that the 
        /// Password object correctly generates password strength metadata about generated passwords.
        /// </summary>
        public void BasicPasswordTest(
            PasswordType passwordType, 
            int passwordLengthInChars, 
            PasswordStrength expectedStrength, 
            double approximateExpectedStrengthInBits)
        {
            Password password = new Password(passwordType, passwordLengthInChars);

            Assert.IsTrue(password.Value.Length == passwordLengthInChars, "The password's length should match the length the user requested.");
            Assert.AreEqual(expectedStrength, actual: password.Strength);

            double actualStengthInBits = Math.Floor(password.StrengthInBits);
            Assert.AreEqual(approximateExpectedStrengthInBits, actual: actualStengthInBits);
        }

        [TestMethod]
        public void PinPasswordsShouldOnlyContainEnglishNumbers()
        {
            PasswordShouldHaveCharacteristics(
                PasswordType.Numeric, 
                isCharacterValid: IsEngishNumber, 
                allowedPasswordCharsDescription: "All PIN characters should be digits.");
        }

        [TestMethod]
        public void AlphaNumericPasswordsShouldOnlyContainEnglishLettersAndNumbers()
        {
            PasswordShouldHaveCharacteristics(
                PasswordType.AlphaNumeric,
                isCharacterValid: IsEngishLetterOrNumber,
                allowedPasswordCharsDescription: "All characters should be letters (a-z, A-Z) or numbers (0-9).");
        }

        [TestMethod]
        public void AnyUSEnglishKeyboardKeyExceptASpacePasswordsShouldOnlyContainEnglishLettersNumbersAndSymbols()
        {
            PasswordShouldHaveCharacteristics(
                PasswordType.AnyKeyOnAnEnglishKeyboardExceptASpace,
                isCharacterValid: character => CanCharacterByTypedOnUSEnglish101KeyKeyboard(character) && !IsSpace(character),
                allowedPasswordCharsDescription: "All characters should be letters (a-z, A-Z), numbers (0-9) or symbols (`~, !, @, \", etc.)");
        }

        [TestMethod]
        public void AnyUSEnglishKeyboardKeyPaswordsShouldOnlyContainEnglishLettersNumbersSymbolsAndSpaces()
        {
            PasswordShouldHaveCharacteristics(
                PasswordType.AnyKeyOnAnEnglishKeyboard,
                isCharacterValid: CanCharacterByTypedOnUSEnglish101KeyKeyboard,
                allowedPasswordCharsDescription: "All characters should be letters (a-z, A-Z), numbers (0-9), symbols (`~, !, @, \", etc.) or a space");
        }

        [TestMethod]
        public void VerifyEachPasswordStrengthHasADifferentDescription()
        {
            VerifyEachPasswordTypeReturnsADifferentPropertyValue(password => password.StrengthDescription);
        }

        [TestMethod]
        public void VerifyEachPasswordDisplayTextIsDifferent()
        {
            VerifyEachPasswordTypeReturnsADifferentPropertyValue(password => password.StrengthDisplayText);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PasswordTooShortTest()
        {
            TestPasswordObjectCreationWhichShouldThrowException(passwordLengthInCharacters: 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PasswordTooLongTest()
        {
            TestPasswordObjectCreationWhichShouldThrowException(passwordLengthInCharacters: Constants.MaximumPasswordLengthInChars + 1);
        }

        private static void PasswordShouldHaveCharacteristics(PasswordType passwordType, Func<char, bool> isCharacterValid, string allowedPasswordCharsDescription)
        {
            const int NumberOfPasswordsToTry = 10000;

            for (int passwordIndex = 0; passwordIndex < NumberOfPasswordsToTry; passwordIndex++)
            {
                const int requestedPasswordLength = Constants.MaximumPasswordLengthInChars;

                Password password = new Password(passwordType, passwordLengthInCharacters: requestedPasswordLength);
                char[] passwordCharacters = password.Value.ToCharArray();

                Trace.WriteLine($"Here are the password's value: {password.Value}");

                Assert.IsTrue(passwordCharacters.Length == requestedPasswordLength, "The length should match the requested password length.");
                Assert.IsTrue(passwordCharacters.All(isCharacterValid), allowedPasswordCharsDescription);
            }
        }

        private static void TestPasswordObjectCreationWhichShouldThrowException(int passwordLengthInCharacters)
        {
            Password password = new Password(PasswordType.AnyKeyOnAnEnglishKeyboard, passwordLengthInCharacters);
            Assert.Fail("This code should never execute because Password's constructors should throw an exception.");
        }

        private static bool IsEngishNumber(char character)
        {
            return character is (>= '0') and (<= '9');
        }

        private static bool IsEngishLetterOrNumber(char character)
        {
            bool isLetter = character is ((>= 'a') and (<= 'z')) or
                                         ((>= 'A') and (<= 'Z'));
            return IsEngishNumber(character) || isLetter;
        }

        private static bool CanCharacterByTypedOnUSEnglish101KeyKeyboard(char character)
        {
            IReadOnlySet<char> symbolsWhichCanBeTypedOnAUS101KeyKeyboard = new HashSet<char>()
                {
                    '`', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+',

                    '[', '{', ']', '}', '\\', '|',

                    ';', ':', '\'', '\"',

                    ',', '<', '.', '>', '/', '?',
                };

            return IsEngishLetterOrNumber(character) ||
                   symbolsWhichCanBeTypedOnAUS101KeyKeyboard.Contains(character) ||
                   IsSpace(character);
        }

        private static bool IsSpace(char character)
        {
            return (character == ' ');
        }

        private static  void VerifyEachPasswordTypeReturnsADifferentPropertyValue(Func<Password, string> getProperty)
        {
            Password[] differentStrengthPasswords = new[]
            {
                // Strong password 
                new Password(PasswordType.AnyKeyOnAnEnglishKeyboard, passwordLengthInCharacters: 22),

                // Adequate Password
                new Password(PasswordType.AlphaNumeric, passwordLengthInCharacters: 8),

                // Adequate PIN password 
                new Password(PasswordType.Numeric, passwordLengthInCharacters: 6),
                
                // Weak password
                new Password(PasswordType.Numeric, 1),
            };

            HashSet<string> previouslySeenPropertyValues = new(StringComparer.InvariantCulture);

            foreach (Password currentPassword in differentStrengthPasswords)
            {
                string propertyValue = getProperty(currentPassword);

                Assert.IsFalse(
                    previouslySeenPropertyValues.Contains(propertyValue),
                    "Each different password type should have a unique property value.");
                previouslySeenPropertyValues.Add(propertyValue);
            }
        }
    }
}
