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

using GenericCommandLineArgumentParser.Properties;
using System.Collections.Generic;
using System.Linq;

namespace GenericCommandLineArgumentParser
{
    public abstract class CommandLineArgumentParser
    {
        public Command ParseCommandLineArguments(string[] args)
        {
            if (args.Length == 0)
            {

            }
            
            const string COMMAND_ARGUMENT_PREFIX = "/";

            // The first command line argument is the command argument.  The command argument selects what the command line program will do (display 
            // help, do A, do B, etc.).
            string commandArgumentFromCommandLine = args[0];
            if (!commandArgumentFromCommandLine.StartsWith(COMMAND_ARGUMENT_PREFIX))
            {
                throw new InvalidCommandLineArgument(ErrorMessages.CommandParameterMissingBackSlashPrefix, commandArgumentFromCommandLine);
            }

            const int FIRST_CHARACTER_AFTER_COMMAND_PREFIX = 1;

            string commandNameWithoutPrefex = commandArgumentFromCommandLine.Substring(FIRST_CHARACTER_AFTER_COMMAND_PREFIX);
            commandNameWithoutPrefex = commandNameWithoutPrefex.ToLower();
            Command command = CommandList.FirstOrDefault(currentCommand => currentCommand.IsName(commandNameWithoutPrefex));
            if (null == command)
            {
                throw new InvalidCommandLineArgument(ErrorMessages.InvalidCommandParameterSpecified, commandArgumentFromCommandLine);
            }

            // The first command line parameter always selects the command to execute.  The rest of the comand line 
            // parameters are arguments for the command.
            string[] commandArguments = args.Skip(1)
                                            .ToArray<string>();
            if (!command.IsNumberOfArgumentsValid(commandArguments.Length))
            {
                throw new InvalidCommandLineArgument(
                    ErrorMessages.InvalidNumberOfCommandLineArgumentsForAParticularCommand,
                    args.Length,
                    checked(command.MinNumberOfArguments + 1), // We add 1 for the command command line parameter.
                    checked(command.MaxNumberOfArguments + 1)); // We add 1 for the command command line parameter.
            }

            bool commandHasArguments = (args.Length > 1);
            if (commandHasArguments)
            {
                command.ParseCommandArguments(commandArguments);
            }

            return command;
        }

        protected abstract IReadOnlyList<Command> CommandList { get; }
    }
}
