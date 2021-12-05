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
using Generator;
using GenericCommandLineArgumentParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using TestUtil;

namespace GeneratorUnitTest
{
    public enum TestStringType
    {
        NotANonNegaiveInteger,
        ZeroOrPositiveInteger
    }

    [TestClass]
    public class GeneratePasswordCommandUnitTests
    {
        // This test tests the error handling code in GeneratePasswordCommand.ParseCommandArguments().  Password commands only have one 
        // argument and that argument is the length of the password.  A valid length is an integer greater than or equal to 1 and less 
        // than or equal to 256.  These tests pass invalid lengths to the parsing code to test the error handling code.
        [DataTestMethod]

        // The empty string is an invalid length because it contains no characters.  
        [DataRow("", TestStringType.NotANonNegaiveInteger)]

        // This string is an invalid length because it is not a number.
        [DataRow("X", TestStringType.NotANonNegaiveInteger)]
        
        // This string is an invalid length because it is not a number.
        [DataRow("Abcdefg", TestStringType.NotANonNegaiveInteger)] 
        
        // -1 is an invalid length because a password cannot have a negative number of characters.
        [DataRow("-1", TestStringType.NotANonNegaiveInteger)]

        // 0 is an invalid password length because a password must have at least 1 character (a 1 character password is 
        // insecure but is it still a valid password).
        [DataRow("0", TestStringType.ZeroOrPositiveInteger)]

        // Passwords cannot have a fractional number of characters (what does it even mean to have a password with 10.4 characters in it?).
        [DataRow("10.4", TestStringType.NotANonNegaiveInteger)]

        // Passwords can have at most 256 characters.
        [DataRow("257", TestStringType.ZeroOrPositiveInteger)]

        // Test the code which hands integer overflows and underflows.  In C#, an int's value can go from -2,147,483,648 to 2,147,483,647 .
        // (https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/int)
        [DataRow("-2147483649", TestStringType.NotANonNegaiveInteger)] 
        [DataRow("2147483648", TestStringType.ZeroOrPositiveInteger)] 

        public void ParseCommandArgumentsShouldThrowAnExceptionWhenThePasswordLengthIsInvalid(string invalidDataLengthString, TestStringType testStringType)
        {
            // This test cannot create an instance of GeneratePasswordCommand because GeneratePasswordCommand is an abstract type.  It 
            // does create an instance of a type derived from GeneratePasswordCommand.  The type of derived class doesn't matter.   
            GeneratePasswordCommand generatePasswordCommand = GeneratePasswordCommand.CreateUsingAnyCharacterOnAKeyboardCommand();

            string expectedExceptionText;

            switch (testStringType)
            {
                case TestStringType.NotANonNegaiveInteger:
                    expectedExceptionText = $"The password length command line parameter contains invalid data because it does not contain a positive integer number.  Here is what the parameter contains: {invalidDataLengthString}.";
                    break;

                case TestStringType.ZeroOrPositiveInteger:
                    expectedExceptionText = $"The password length command line parameter contains a number which is either too low or too high.  This program can generate a password which has {Constants.MinimumPasswordLengthInChars} and {Constants.MaximumPasswordLengthInChars} characters (inclusive).  Here is the parameter's value: {invalidDataLengthString}.";
                    break;

                default:
                    throw new Exception("This case should never occur.");
            }

            TestHelper.TestActionWhichShouldThrowAnException<InvalidCommandLineArgumentException>(
                () => generatePasswordCommand.ParseCommandArguments(new string[] { invalidDataLengthString }),
                expectedExceptionText);
        }

        [DataTestMethod]

        // The shortest possible PIN (not recommended)  
        [DataRow(1)]

        // Common PIN lengths
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]

        // The longest possible PIN 
        [DataRow(256)]

        public void TestGeneratePinCommandClass(int validPasswordLength)
        {
            GeneratePasswordCommand generatePinCommand = GeneratePasswordCommand.CreatePinCommand();
            string[] commandArguments = { validPasswordLength.ToString(CultureInfo.InvariantCulture) };
            generatePinCommand.ParseCommandArguments(commandArguments);

            // The test passes if there are no exceptions.
            generatePinCommand.Run();
        }
    }
}
