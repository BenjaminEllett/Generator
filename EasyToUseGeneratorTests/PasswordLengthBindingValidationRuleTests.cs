using EasyToUseGenerator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Windows.Controls;
using TestUtil;

namespace EasyToUseGeneratorTests
{
    [TestClass()]
    public class PasswordLengthBindingValidationRuleTests
    {
        // The lengths were chosen because they are common password lengths.
        [DataTestMethod()]
        [DataRow("1")] // Minimum password length
        [DataRow("4")]
        [DataRow(" 4")] // A password with a leading white space character
        [DataRow("6")]
        [DataRow("6 ")] // A password with a trailing white space character
        [DataRow("8")]
        [DataRow(" 8 ")] // A password with a leading and trailing white space character
        [DataRow("8")]
        [DataRow("10")]
        [DataRow("22")]
        [DataRow("   22")] // A password with multiple leading white space characters
        [DataRow("22  ")] // A password with multiple trailing white space characters
        [DataRow("        22      ")] // A password with multiple leading and trailing white space characters
        [DataRow("24")]
        [DataRow("38")] // Largest supported password length

        public void ValidateShould_Accept_ValidPasswordLengths(string validPasswordLength)
        {
            ValidationResult result = ValidatePasswordLength(validPasswordLength);
            Assert.IsTrue(result.IsValid, "The result should be valid because a valid input was passed to the Validate() method");
            Assert.IsNull(result.ErrorContent, "ErrorContent should be null because IsValid() returned true");
        }

        [DataTestMethod()]

        // The empty string is an invalid length because it contains no characters.  
        [DataRow("")]

        // This string is an invalid length because it is not a number.
        [DataRow("X")]

        // This string is an invalid length because it is not a number.
        [DataRow("Abcdefg")]

        // -1 is an invalid length because a password cannot have a negative number of characters.
        [DataRow("-1")]

        // 0 is an invalid password length because a password must have at least 1 character (a 1 character password is 
        // insecure but is it still a valid password).
        [DataRow("0")]

        // Passwords cannot have a fractional number of characters (what does it even mean to have a password with 10.4 characters in it?).
        [DataRow("10.4")]

        // The GUI version of the application supports at most 38 characters in a password.
        [DataRow("39")]

        // Passwords can have at most 256 characters.
        [DataRow("257")]

        // Test the code which hands integer overflows and underflows.  In C#, an int's value can go from -2,147,483,648 to 2,147,483,647 .
        // (https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/int)
        [DataRow("-2147483649")]
        [DataRow("2147483648")]
        public void ValidateShould_Reject_InvalidPasswordLengths(string invalidPasswordLength)
        {
            ValidationResult result = ValidatePasswordLength(invalidPasswordLength);
            Assert.IsFalse(result.IsValid, "The result should be invalid because a valid password length was NOT passed to the Validate() method");
            Assert.IsNotNull(result.ErrorContent, "ErrorContent should NOT be null because IsValid() returned false");
            Assert.IsNotNull(result.ErrorContent.GetType() == typeof(string), "ErrorContent should contain a string which can be displayed to the user.");

            string errorMessage = (string)result.ErrorContent;
            Assert.IsFalse(string.IsNullOrWhiteSpace(errorMessage), "The error message should contain a string which tells the user how to correct the error.");
        }

        [TestMethod]
        public void ValidShouldThrowAnExceptionIfAnUnsupportedTypeIsPassedToIt()
        {
            byte unsupportedType = 1;

            TestHelper.TestActionWhichShouldThrowAnException<ArgumentException>(
                () => ValidatePasswordLength(unsupportedType),
                expectedExceptionText: "This class can only validate strings.");
        }

        private static ValidationResult ValidatePasswordLength<T>(T potentialPasswordLength)
            where T : notnull
        {
            var passwordLengthBindingValidationRule = new PasswordLengthBindingValidationRule();
            return passwordLengthBindingValidationRule.Validate(potentialPasswordLength, CultureInfo.InvariantCulture);
        }
    }
}