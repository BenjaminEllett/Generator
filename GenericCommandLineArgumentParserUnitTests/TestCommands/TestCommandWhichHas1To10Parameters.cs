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

namespace GenericCommandLineArgumentParserUnitTests.TestCommands
{
    /// <summary>
    /// TestCommandWhichHas1To10Parameters is a command which has 1 to 10 parameters.  It is equivalent to typing the following commands on the command line:
    /// 
    /// CommandLineApp.exe /TC1-10P 1 
    /// CommandLineApp.exe /TC1-10P 1 2 
    /// CommandLineApp.exe /TC1-10P 1 2 3 
    /// CommandLineApp.exe /TC1-10P 1 2 3 4
    /// CommandLineApp.exe /TC1-10P 1 2 3 4 5
    /// CommandLineApp.exe /TC1-10P 1 2 3 4 Five Six 
    /// CommandLineApp.exe /TC1-10P One 2 3 4 5 Six Seven
    /// CommandLineApp.exe /TC1-10P One 2 3 4 5 Six Seven 8 
    /// CommandLineApp.exe /TC1-10P One 2 3 4 5 Six Seven 8 9
    /// CommandLineApp.exe /TC1-10P One 2 3 4 5 Six Seven 8 9 Ten
    ///
    /// CommandLineApp.exe /TestCommandWhichHas1To10Parameters 1 
    /// CommandLineApp.exe /TestCommandWhichHas1To10Parameters 1 2 
    /// CommandLineApp.exe /TestCommandWhichHas1To10Parameters 1 2 3 
    /// CommandLineApp.exe /TestCommandWhichHas1To10Parameters 1 2 3 4
    /// CommandLineApp.exe /TestCommandWhichHas1To10Parameters 1 2 3 4 5
    /// CommandLineApp.exe /TestCommandWhichHas1To10Parameters 1 2 3 4 Five Six 
    /// CommandLineApp.exe /TestCommandWhichHas1To10Parameters One 2 3 4 5 Six Seven
    /// CommandLineApp.exe /TestCommandWhichHas1To10Parameters One 2 3 4 5 Six Seven 8 
    /// CommandLineApp.exe /TestCommandWhichHas1To10Parameters One 2 3 4 5 Six Seven 8 9
    /// CommandLineApp.exe /TestCommandWhichHas1To10Parameters One 2 3 4 5 Six Seven 8 9 Ten
    /// 
    /// </summary>
    public class TestCommandWhichHas1To10Parameters : TestCommand
    {
        public TestCommandWhichHas1To10Parameters() :
            base(shortCommandParameterName: "TC1-10P",
                 longCommandParameterName: "TestCommandWhichHas1To10Parameters",
                 minNumberOfArguments: 1,
                 maxNumberOfArguments: 10)
        {
        }
    }
}
