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

using Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GeneratorUnitTest
{
    [TestClass]
    public class PasswordUnitTests
    {
        [DataTestMethod]

        // Test the lower boundary condition - Passwords with 1 character.
        [DataRow(PasswordType.Numeric, 1, PasswordStrength.Weak, 3.0)]
        [DataRow(PasswordType.AlphaNumericPassword, 1, PasswordStrength.Weak, 5.0)]
        [DataRow(PasswordType.AnyKeyOnAnEnglishKeyboard, 1, PasswordStrength.Weak, 6.0)]

        // Test the common PIN lengths
        [DataRow(PasswordType.Numeric, 4, PasswordStrength.Weak, 13.0)]
        [DataRow(PasswordType.Numeric, 6, PasswordStrength.Weak, 19.0)]

        // Test common password lengths 
        [DataRow(PasswordType.AlphaNumericPassword, 16, PasswordStrength.Weak, 95.0)] // Unfortunately, Office 365 limits passwords to 16 characters
        [DataRow(PasswordType.AlphaNumericPassword, 20, PasswordStrength.Weak, 119.0)]
        [DataRow(PasswordType.AlphaNumericPassword, 24, PasswordStrength.AdequateForProtectingAllOfYourData, 142.0)]

        [DataRow(PasswordType.AnyKeyOnAnEnglishKeyboard, 16, PasswordStrength.Weak, 105.0)] // Unfortunately, Office 365 limits passwords to 16 characters
        [DataRow(PasswordType.AnyKeyOnAnEnglishKeyboard, 20, PasswordStrength.AdequateForProtectingAllOfYourData, 131.0)]
        [DataRow(PasswordType.AnyKeyOnAnEnglishKeyboard, 24, PasswordStrength.AdequateForProtectingAllOfYourData, 157.0)]

        // Test the upper boundary condition - Passwords with the maximum number of characters (256).

        [DataRow(PasswordType.Numeric, 256, PasswordStrength.AdequateForProtectingAllOfYourData, 850.0)] 
        [DataRow(PasswordType.AlphaNumericPassword, 256, PasswordStrength.AdequateForProtectingAllOfYourData, 1524.0)]
        [DataRow(PasswordType.AnyKeyOnAnEnglishKeyboard, 256, PasswordStrength.AdequateForProtectingAllOfYourData, 1681.0)]

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
        [ExpectedException(typeof(ArgumentException))]
        public void PasswordTooShortTest()
        {
            TestPasswordObjectCreationWhichShouldThrowException(passwordLengthInCharacters: 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PasswordTooLongTest()
        {
            TestPasswordObjectCreationWhichShouldThrowException(passwordLengthInCharacters: 257);
        }

        private static void TestPasswordObjectCreationWhichShouldThrowException(int passwordLengthInCharacters)
        {
            Password password = new Password(PasswordType.AnyKeyOnAnEnglishKeyboard, passwordLengthInCharacters);
            Assert.Fail("This code should never execute because Password's constructors should throw an exception.");
        }
    }
}
