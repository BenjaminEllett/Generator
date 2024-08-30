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
using EasyToUseGenerator.Resources;
using System;
using System.Globalization;
using System.Windows.Controls;

namespace EasyToUseGenerator
{
    public class PasswordLengthBindingValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (typeof(string) != value.GetType())
            {
                throw new ArgumentException("This class can only validate strings.", paramName: nameof(value));
            }

            int potentialNewLength;

            if (!int.TryParse(
                    (string)value, 
                    NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, 
                    cultureInfo, 
                    out potentialNewLength))
            {
                return CreateInvalidPasswordLengthResult();
            }

            if (!Password.IsValidPasswordLength(potentialNewLength))
            {
                return CreateInvalidPasswordLengthResult();
            }

            // This application only supports passwords which have between 1 and 38 characters.  The reason
            // is the application's UI cannot display passwords longer than 38 charactes and there is no 
            // good reason to support longer passwords.  A non-numeric password which is 22 characters long
            // has more than 2^128 possible values.  It is unfeasible to guess these passwords with current
            // technology.  There is no benefit to having a 39 or more character password because a 38
            // character password is already unguessable.
            const int MaxSupportedPasswordLengthInThisApplication = 38;

            if (potentialNewLength > MaxSupportedPasswordLengthInThisApplication)
            {
                return CreateInvalidPasswordLengthResult();
            }

            return ValidationResult.ValidResult;



            ValidationResult CreateInvalidPasswordLengthResult()
            {
                return new ValidationResult(isValid: false, errorContent: ErrorMessages.InvalidNewPasswordLength);
            }
        }
    }
}
