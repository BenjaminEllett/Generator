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

using System;
using System.Diagnostics;

namespace GenericCommandLineArgumentParser
{
    public abstract class Command
    {
        public Command(
            string shortCommandParameterName,
            string longCommandParameterName,
            uint minNumberOfArguments,
            uint maxNumberOfArguments
            )
        {
            Debug.Assert(shortCommandParameterName.Length < longCommandParameterName.Length,
                         "The long command name should always be longer than the short command name.");

            Debug.Assert(minNumberOfArguments <= maxNumberOfArguments,
                         "The minimum number of arguments must be less than or equal to the maximum number of arguments.");
            
            this.shortCommandParameterName = shortCommandParameterName;
            this.longCommandParameterName = longCommandParameterName;
            MinNumberOfArguments = minNumberOfArguments;
            MaxNumberOfArguments = maxNumberOfArguments;
        }

        public uint MinNumberOfArguments { get; private init; }

        public uint MaxNumberOfArguments { get; private init; }

        public bool IsName(string commandNameWithoutPrefex)
        {
            return string.Equals(commandNameWithoutPrefex, this.shortCommandParameterName, StringComparison.CurrentCultureIgnoreCase) ||
                   string.Equals(commandNameWithoutPrefex, this.longCommandParameterName, StringComparison.CurrentCultureIgnoreCase);
        }

        public bool IsNumberOfArgumentsValid(int numberOfArguments)
        {
            return (MinNumberOfArguments <= ((uint)numberOfArguments)) && (((uint)numberOfArguments) <= MaxNumberOfArguments);
        }

        public abstract void ParseCommandArguments(string[] commandsArguments);
        public abstract void Run();

        private readonly string shortCommandParameterName;
        private readonly string longCommandParameterName;
    }
}


