//
// MIT License
//
// Copyright(c) 2019-2024 Benjamin Ellett
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
using GenericCommandLineArgumentParser;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Generator
{
    public class GeneratePasswordCommand : Command
    {
        private PasswordType passwordType;
        private int? passwordLengthInCharacters;

        public static GeneratePasswordCommand CreatePinCommand()
        {
            return new GeneratePasswordCommand(
                shortCommandParameterName: "PIN",
                longCommandParameterName: "GeneratePersonalIdentificationNumber",
                PasswordType.Numeric);
        }

        public static GeneratePasswordCommand CreateHexCommand()
        {
            return new GeneratePasswordCommand(
                shortCommandParameterName: "HEX",
                longCommandParameterName: "GenerateHexString",
                PasswordType.Hex);
        }

        public static GeneratePasswordCommand CreateUsingAnyCharacterOnAKeyboardCommand()
        {
            return new GeneratePasswordCommand(
                shortCommandParameterName: "AKB",
                longCommandParameterName: "GeneratePasswordUsingAnyCharacterOnAKeyboard",
                PasswordType.AnyKeyOnAnEnglishKeyboard);
        }

        public GeneratePasswordCommand(string shortCommandParameterName, string longCommandParameterName, PasswordType passwordType) :
            base(shortCommandParameterName,
                 longCommandParameterName,
                 minNumberOfArguments: 1,
                 maxNumberOfArguments: 1)
        {
            this.passwordLengthInCharacters = null;
            this.passwordType = passwordType;
        }

        public override void ParseCommandArguments(string[] commandsArguments)
        {
            if (commandsArguments == null)
            {
                throw new ArgumentNullException(nameof(commandsArguments));
            }

            Debug.Assert(
                1 == commandsArguments.Length,
                "Generate password commands only have one argument.");

            const uint PasswordLengthArgumentIndex = 0;

            int passwordLengthInCharacters;
            string passwordLengthParameter = commandsArguments[PasswordLengthArgumentIndex];

            try
            {
                // Parse the password length command line parameter.  This parameter's
                // type is a integer number.
                passwordLengthInCharacters = int.Parse(passwordLengthParameter, NumberStyles.None, CultureInfo.CurrentCulture);
            }
            catch (FormatException e)
            {
                throw new InvalidCommandLineArgumentException(
                    CommonErrorMessages.PasswordLengthDoesNotContainAValidNumber,
                    passwordLengthParameter,
                    e);
            }
            catch (OverflowException e)
            {
                throw new InvalidCommandLineArgumentException(
                    CommonErrorMessages.PasswordLengthOutOfRangeFormatString,
                    Constants.MinimumPasswordLengthInChars,
                    Constants.MaximumPasswordLengthInChars,
                    passwordLengthParameter,
                    e);
            }

            if (!Password.IsValidPasswordLength(passwordLengthInCharacters))
            {
                throw new InvalidCommandLineArgumentException(
                    CommonErrorMessages.PasswordLengthOutOfRangeFormatString,
                    Constants.MinimumPasswordLengthInChars,
                    Constants.MaximumPasswordLengthInChars,
                    passwordLengthParameter);
            }

            this.passwordLengthInCharacters = passwordLengthInCharacters;
        }

        public override void Run()
        {
            // ParseCommandArguments() should always be called before Run().
            if (!passwordLengthInCharacters.HasValue)
            {
                throw new Exception(CommonErrorMessages.ThisCaseShouldNeverOccur);
            }

            Password newPassword = new Password(this.passwordType, this.passwordLengthInCharacters.Value);

            Console.WriteLine(UserInterface.NewPassword, newPassword.Value);
            Console.WriteLine();
            Console.WriteLine(UserInterface.NewPasswordStrengthInBits, newPassword.StrengthInBits);
            Console.WriteLine();
            Console.WriteLine(UserInterface.NewPasswordStrength, newPassword.StrengthDescription);
        }
    }
}
