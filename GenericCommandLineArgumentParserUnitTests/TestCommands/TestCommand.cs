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

using GenericCommandLineArgumentParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace GenericCommandLineArgumentParserUnitTests.TestCommands
{
    public class TestCommand : Command
    {
        public TestCommand(
            string shortCommandParameterName, 
            string longCommandParameterName, 
            uint minNumberOfArguments, 
            uint maxNumberOfArguments) : 
                base(
                    shortCommandParameterName, 
                    longCommandParameterName,
                    minNumberOfArguments,
                    maxNumberOfArguments)
        {
        }

        public bool RunCalled { get; private set; } = false;
        public bool ParseCommandArgumentsCalled { get; private set; } = false;
        public IReadOnlyList<string>? CommandsArguments { get; private set; } = null;

        public override void Run()
        {
            RunCalled = true;
            // Test commands don't do anything.
        }

        public override void ParseCommandArguments(string[] commandsArguments)
        {
            ParseCommandArgumentsCalled = true;
            CommandsArguments = new List<string>(commandsArguments);

            Assert.IsTrue((MinNumberOfArguments <= commandsArguments.Length) &&
                          (commandsArguments.Length <= MaxNumberOfArguments),
                          "The base class should not call this function if there are an invalid number of command arguments.");
        }
    }
}
