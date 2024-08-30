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
using EasyToUseGenerator;
using System;
using System.Diagnostics.CodeAnalysis;

namespace EasyToUseGeneratorTests.Mocks
{
    internal class MockTextFileService : ITextFileService
    {
        private readonly string? stringTextFileContent;
        private string lastTextPassedToWriteTextFile = string.Empty;

        public MockTextFileService(string? stringTextFileContent = null)
        {
            this.stringTextFileContent = stringTextFileContent;
        }

        public void CreateDirectoryIfItDoesNotExist(string directoryPath)
        {
            NumTimesCreateDirectoryIfItDoesNotExistCalled++;
        }

        public bool TryReadTextFile(string filePath, [NotNullWhen(true)] out string? stringTextFileContent)
        {
            stringTextFileContent = this.stringTextFileContent;
            return (null != this.stringTextFileContent);
        }

        public void WriteTextFile(string filePath, string fileContent)
        {
            this.lastTextPassedToWriteTextFile = fileContent;
            NumTimesWriteTextFileCalled++;
        }

        public int NumTimesCreateDirectoryIfItDoesNotExistCalled { get; private set; } = 0;
        public int NumTimesWriteTextFileCalled { get; private set; } = 0;
        public string LastTextPassedToWriteTextFile 
        { 
            get
            {
                if (0 >= this.NumTimesWriteTextFileCalled)
                {
                    throw new InvalidOperationException("Cannot return the last text passsed to WriteTextFile() because WriteTextFile() was never called.");
                }

                return this.lastTextPassedToWriteTextFile;
            }
        }
    }
}
