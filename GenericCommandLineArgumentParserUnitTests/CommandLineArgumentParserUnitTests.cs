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

using GenericCommandLineArgumentParser;
using GenericCommandLineArgumentParserUnitTests.TestArgumentParsers;
using GenericCommandLineArgumentParserUnitTests.TestCommands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using TestUtil;

namespace GenericCommandLineArgumentParserUnitTests
{
    [TestClass]
    public class CommandLineArgumentParserUnitTests
    {
        //
        // Valid Input Tests
        //
        // These tests make sure CommandLineArgumentParser can parse command lines and run commands.  These tests 
        // ensure that the class works when it processes valid inputs.
        //

        [DataTestMethod]

        // Test the edge case where a derived class only has one command and that command has no parameters.
        [DataRow(new string[] { "/TC0" }, typeof(TestCommandWithNoParameters))]
        [DataRow(new string[] { "/TestCommandWithNoParameters" }, typeof(TestCommandWithNoParameters))]

        public void ValidInputCommandLineArgumentParserEdgeCaseTests(string[] commandLineArgs, Type expectedCommandType)
        {
            var testCommandLineArgumentParser1 = new TestCommandLineArgumentParser1();
            TestCommandLineArgumentParserWorksWithValidInput(testCommandLineArgumentParser1, commandLineArgs, expectedCommandType);
        }

        [DataTestMethod]

        // Test the case where a derived class has many commands

        // The the case where a command accepts exactly one command line parameter
        [DataRow(new string[] { "/TC1", "CommandParameter" }, typeof(TestCommandWithExactly_1_Parameter))]
        [DataRow(new string[] { "/TestCommandWithExactly_1_Parameter", "CommandParameter" }, typeof(TestCommandWithExactly_1_Parameter))]

        // The the case where a command accepts 1 to 10 command line parameters
        [DataRow(new string[] { "/TC1-10P", "1" }, typeof(TestCommandWhichHas1To10Parameters))]
        [DataRow(new string[] { "/TC1-10P", "1", "2" }, typeof(TestCommandWhichHas1To10Parameters))]
        [DataRow(new string[] { "/TC1-10P", "1", "2", "3" }, typeof(TestCommandWhichHas1To10Parameters))]
        [DataRow(new string[] { "/TC1-10P", "1", "2", "3", "4" }, typeof(TestCommandWhichHas1To10Parameters))]
        [DataRow(new string[] { "/TC1-10P", "1", "2", "3", "4", "5" }, typeof(TestCommandWhichHas1To10Parameters))]
        [DataRow(new string[] { "/TC1-10P", "1", "2", "3", "4", "Five", "Six" }, typeof(TestCommandWhichHas1To10Parameters))]
        [DataRow(new string[] { "/TC1-10P", "One", "2", "3", "4", "5", "Six", "Seven" }, typeof(TestCommandWhichHas1To10Parameters))]
        [DataRow(new string[] { "/TC1-10P", "One", "2", "3", "4", "5", "Six", "Seven", "8" }, typeof(TestCommandWhichHas1To10Parameters))]
        [DataRow(new string[] { "/TC1-10P", "One", "2", "3", "4", "5", "Six", "Seven", "8", "9" }, typeof(TestCommandWhichHas1To10Parameters))]
        [DataRow(new string[] { "/TC1-10P", "One", "2", "3", "4", "5", "Six", "Seven", "8", "9", "Ten" }, typeof(TestCommandWhichHas1To10Parameters))]

        // Test the long version of the command line parameter
        [DataRow(new string[] { "/TestCommandWhichHas1To10Parameters", "One", "2", "3", "4", "5", "Six", "Seven", "8", "9", "Ten" }, typeof(TestCommandWhichHas1To10Parameters))]

        public void ValidInputCommandLineArgumentParserTests(
            string[] commandLineArgs,
            Type expectedCommandType)
        {
            var testCommandLineArgumentParser2 = new TestCommandLineArgumentParser2();
            TestCommandLineArgumentParserWorksWithValidInput(testCommandLineArgumentParser2, commandLineArgs, expectedCommandType);
        }



        //
        // Invalid Input Tests
        //

        [DataTestMethod]

        [DataRow(
            new string[0],
            "The user did not specify any command line arguments.")]

        // This data is invalid because the command (/CommandWhichDoesNoteExist) does not exist.
        [DataRow(
            new string[] { "/CommandWhichDoesNoteExist", "A", "B", "C" }, 
            "The command parameter is not valid because it does not correspond to a valid command.  Here is the invalid command parameter: " +
            "/CommandWhichDoesNoteExist")]

        // This data is invalid because the command (CommandWithNoPrefixCharacter) does not start with the command prefix character (/).  
        // All commands are required to start with a / character.
        [DataRow(
            new string[] { "CommandWithNoPrefixCharacter", "A", "B", "C" },
            "The user did not specify a valid command parameter because the command parameter did not start with a / character.  A " +
            "command parameter is the first parameter passed to the program (i.e. the parameter which comes right after the program's " +
            "name on the command line).  Here is the invalid command parameter: CommandWithNoPrefixCharacter")]

        // This data is invalid because command line has a valid command but the command has too few arguments.
        [DataRow(
            new string[] { "/TC1-10P" },
            "The user passed 1 command line parameters to the program but the specified command only has between 2 and 11 command line parameters (this count includes the command parameter)")]

        // This data is invalid because command line has a valid command but the command has too many arguments.
        [DataRow(
            new string[] { "/TC1-10P", "One", "2", "3", "4", "5", "Six", "Seven", "8", "9", "Ten", "11" },
            "The user passed 12 command line parameters to the program but the specified command only has between 2 and 11 command line parameters (this count includes the command parameter)")]

        public void ParseCommandLineArgumentsShouldThrowAnExceptionWhenItParsesInvalidData(
            string[] invalidCommandLineArguments,
            string expectedExceptionText)
        {
            var testCommandLineArgumentParser2 = new TestCommandLineArgumentParser2();

            TestHelper.TestActionWhichShouldThrowAnException<InvalidCommandLineArgumentException>(
                () => testCommandLineArgumentParser2.ParseCommandLineArguments(invalidCommandLineArguments),
                expectedExceptionText);
        }




        private static void TestCommandLineArgumentParserWorksWithValidInput(
            CommandLineArgumentParser commandLineArgumentParser,
            string[] commandLineArgs,
            Type expectedCommandType)
        {
            Command command = commandLineArgumentParser.ParseCommandLineArguments(commandLineArgs);

            Type commandType = command.GetType();
            string assertFailureMessage =
                $"The expected command type did not match the actual command type.  Expected command type: " +
                $"'{expectedCommandType.FullName}'   Actual command type: '{commandType.FullName}'";

            Assert.IsTrue(expectedCommandType == commandType, assertFailureMessage);

            // All of the commands in the tests should derive from the TestCommand class.
            TestCommand testCommand = (TestCommand)command;

            if (commandLineArgs.Length > 1)
            {
                Assert.IsTrue(
                    testCommand.ParseCommandArgumentsCalled,
                    "ParseCommandLineArguments() should be called if two or more valid command line arguments are passed to ParseCommandLineArguments().");

                // Skip the command argument
                IEnumerable<string> expectedCommandArguments = commandLineArgs.Skip(1);

                Assert.IsTrue(
                    (testCommand.CommandsArguments != null),
                    "ParseCommandLineArguments() should be called and this value should contain a list of parameters passed to ParseCommandLineArguments().");

                Assert.IsTrue(
                    testCommand.CommandsArguments!.SequenceEqual(expectedCommandArguments),
                    "The actual command arguments passed to ParseCommandLineArguments() should match the expected command arguments.");
            }
            else
            {
                Assert.IsFalse(
                    testCommand.ParseCommandArgumentsCalled,
                    "ParseCommandLineArguments() should NOT be called if only one command line argument is passed to ParseCommandLineArguments()");
            }
            
            Assert.IsFalse(
                testCommand.RunCalled,
                "Run should not be called if a valid command line is passed to ParseCommandLineArguments()");
        }
    }
}
