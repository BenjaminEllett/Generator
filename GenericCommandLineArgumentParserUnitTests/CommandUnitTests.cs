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

using GenericCommandLineArgumentParserUnitTests.TestCommands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericCommandLineArgumentParserUnitTests
{
    [TestClass]
    public class CommandUnitTests
    {
        public CommandUnitTests()
        {
            this.testCommand = new TestCommand(
                shortCommandParameterName: "TC",
                longCommandParameterName: "TestCommand",
                minNumberOfArguments: MinNumberOfArguments,
                maxNumberOfArguments: MaxNumberOfArguments);
        }

        [TestMethod]
        public void ConstructorShouldInitialize_MinNumberOfArguments_Property()
        {
            Assert.IsTrue(
                this.testCommand.MinNumberOfArguments == MinNumberOfArguments,
                "If this assert fires, it means that that value passed to the constructor's minNumberOfArguments parameter is not the " +
                "value of the class's MinNumberOfArguments property.  This is a bug.");
        }

        [TestMethod]
        public void ConstructorShouldInitialize_MaxNumberOfArguments_Property()
        {
            Assert.IsTrue(
                this.testCommand.MaxNumberOfArguments == MaxNumberOfArguments,
                "If this assert fires, it means that that value passed to the constructor's maxNumberOfArguments parameter is not the " +
                "value of the class's MaxNumberOfArguments property.  This is a bug.");
        }

        [DataTestMethod]
        [DataRow("TC")]
        [DataRow("tc")]
        [DataRow("Tc")]
        [DataRow("TESTCOMMAND")]
        [DataRow("testcommand")]
        [DataRow("TestCommand")]
        public void IsNameShouldReturnTrueWhenTheCommandNameIsPassedToIt(string validCommandName)
        {
            Assert.IsTrue(this.testCommand.IsName(validCommandName));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("X")] // X is not the name of the command
        [DataRow("NotTheNameOfTheCommand")]
        public void IsNameShouldReturn_False_WhenTheCommandNameIs_Not_PassedToIt(string invalidCommandName)
        {
            Assert.IsFalse(this.testCommand.IsName(invalidCommandName));
        }

        /// <summary>
        /// IsNumberOfArgumentsValidTests() tests Command.IsNumberOfArgumentsValid().  It passes valid values to 
        /// IsNumberOfArgumentsValid() and makes sure IsNumberOfArgumentsValid() returns true.  Valid values are 
        /// values between the minumum and maxium number of arguments a command can take (inclusive).
        /// </summary>
        [DataTestMethod]
        [DataRow((int)MinNumberOfArguments)]
        [DataRow(100)] // This value is inbetween the minimum and maxium number of valid arguments.
        [DataRow((int)MaxNumberOfArguments)]
        public void IsNumberOfArgumentsValidTests(int validNumberOfArguments)
        {
            Assert.IsTrue(this.testCommand.IsNumberOfArgumentsValid(validNumberOfArguments));
        }

        /// <summary>
        /// IsNumberOfArgumentsValidTests() tests Command.IsNumberOfArguments().  It passes invalid values to 
        /// IsNumberOfArgumentsValid() and makes sure IsNumberOfArgumentsValid() returns false.  
        /// </summary>
        [DataTestMethod]
        [DataRow(((int)MinNumberOfArguments) - 1)]
        [DataRow(((int)MaxNumberOfArguments) + 1)]
        public void IsNumberOfArguments_Invalid_Tests(int invalidNumberOfArguments)
        {
            Assert.IsFalse(this.testCommand.IsNumberOfArgumentsValid(invalidNumberOfArguments));
        }

        private readonly TestCommand testCommand;

        private const uint MinNumberOfArguments = 10;
        private const uint MaxNumberOfArguments = 999;
    }
}

