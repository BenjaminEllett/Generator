//
// MIT License
//
// Copyright(c) 2019-2022 Benjamin Ellett
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

using EasyToUseGenerator.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Documents;
using TestUtil;

namespace EasyToUseGeneratorTests
{
    [TestClass]
    public class SimpleDocumentServiceTests
    {
        private const string testSectionBody = "0123456789 0123456789 0123456789 0123456789 0123456789";

        private static readonly DocumentSection onlySection = CreateTestSection("Only Section Header");
        private static readonly DocumentSection sectionOne = CreateTestSection(1);
        private static readonly DocumentSection sectionTwo = CreateTestSection(2);

        // Cdwms stands for CreateDocumentWithMultipleSections
        [TestMethod]
        public void CdwmsThrowsAnExceptionIfADocumentHasNoSections()
        {
            TestHelper.TestActionWhichShouldThrowAnException<ArgumentException>(
                PassAnEmptyListOfDocumentSectionsToCdwms,
                "A document must have at least one section.");
            return;

            static void PassAnEmptyListOfDocumentSectionsToCdwms()
            {
                List<DocumentSection> documentSections = new();
                CreateDocumentWithMultipleSections(documentSections);
            }
        }

        [TestMethod]
        public void CdwmsShouldBeAbleToCreateADocumentWithOneSection()
        {
            List<DocumentSection> documentSections = new()
            {
                onlySection,
            };

            FlowDocument flowDocument = CreateDocumentWithMultipleSections(documentSections);

            Assert.IsTrue(
                DoesDocumentContainSection(flowDocument, onlySection),
                "The document should contain the Only Section.");
            Assert.IsTrue(
                GetSectionCount(flowDocument) == 1,
                "The document should only contain 1 section.");
        }

        [TestMethod]
        public void CdwmsShouldBeAbleToCreateADocumentWithTwoSections()
        {
            List<DocumentSection> documentSections = new()
            {
                sectionOne,
                sectionTwo,
            };

            FlowDocument documentWithMultipleSections = CreateDocumentWithMultipleSections(documentSections);

            Assert.IsTrue(
                DoesDocumentContainSection(documentWithMultipleSections, sectionOne),
                "The document should contain Section One.");
            Assert.IsTrue(
                DoesDocumentContainSection(documentWithMultipleSections, sectionTwo),
                "The document should contain Section Two.");
            Assert.IsTrue(
                GetSectionCount(documentWithMultipleSections) == 2,
                "The document should contain 2 sections.");
        }
        
        private static DocumentSection CreateTestSection(string header) =>
            new DocumentSection(header, testSectionBody);

        private static DocumentSection CreateTestSection(int headerNumber) =>
            CreateTestSection($"Section Header {headerNumber}");

        private static FlowDocument CreateDocumentWithMultipleSections(IEnumerable<DocumentSection> documentSections)
        {
            ISimpleDocumentService simpleDocumentService = new SimpleDocumentService();
            return simpleDocumentService.CreateDocumentWithMultipleSections(documentSections);
        }

        private static Paragraph GetFlowDocumentRoot(FlowDocument flowDocument)
        {
            Assert.IsTrue(
                flowDocument.Blocks.Count == 1,
                "The document should have only one root element.  If this changes, this function needs to be revised.");

            IList blocks = (IList)flowDocument.Blocks;
            return (Paragraph)blocks[0]!;
        }

        private static Run GetFirstSection(FlowDocument simpleDocumentWithSections)
        {
            Paragraph paragraph = GetFlowDocumentRoot(simpleDocumentWithSections);
            Inline possibleFirstSectionHeader = paragraph.Inlines.FirstInline;
            Assert.IsTrue(
                IsSection(possibleFirstSectionHeader), 
                "A simple document should only have a list of sections.");
            return (Run)possibleFirstSectionHeader;
        }

        private static bool TryGetNextSection(Run currentSectionHeader, [NotNullWhen(returnValue: true)] out Run? nextSectionHeader)
        {
            nextSectionHeader = default(Run);

            Run currentSectionBody = GetBodyFromSectionHeader(currentSectionHeader);
            if (currentSectionBody.NextInline == null)
            {
                // There is no next inline so there is no next section.
                return false;
            }

            int lineBreakCount = 0;
            const int ExpectedNumberOfLineBreaksBetweenSections = 3;
            Inline? currentInline = currentSectionBody.NextInline;

            while (lineBreakCount < ExpectedNumberOfLineBreaksBetweenSections)
            {
                // null means there is no previous inline
                Assert.IsNotNull(
                    currentInline,
                    "There should always be three line breaks between sections.  The last section does not have any line breaks after it.");

                Assert.IsTrue(
                    currentInline is LineBreak,
                    "Only LineBreak objects are allowed between sections.  The reason is they provide space between sections.s");

                currentInline = currentInline.NextInline;
                lineBreakCount++;
            }

            Assert.IsTrue(
                IsHeader(currentInline),
                "A section header must follow the three line breaks which separate sections.");

            nextSectionHeader = (Run)currentInline;
            return true;
        }

        private static Run GetBodyFromSectionHeader(Run sectionHeader)
        {
            Assert.IsTrue(
                IsBody(sectionHeader.NextInline),
                "A section body must come after the section header.");

            return (Run)sectionHeader.NextInline;
        }

        private static bool DoesDocumentContainSection(FlowDocument simpleDocumentWithSections, DocumentSection section)
        {
            Run? currentSectionHeader = GetFirstSection(simpleDocumentWithSections);

            do
            {
                if (IsExpectedSection(currentSectionHeader, section))
                {
                    return true;
                }

            } while (TryGetNextSection(currentSectionHeader, out currentSectionHeader));
            
            return false;
        }

        private static int GetSectionCount(FlowDocument simpleDocumentWithSections)
        {
            int sectionCount = 0;
            Run? currentSectionHeader = GetFirstSection(simpleDocumentWithSections);

            do
            {
                sectionCount++;

            } while (TryGetNextSection(currentSectionHeader, out currentSectionHeader));

            return sectionCount;
        }

        private static bool IsSection(Inline inline)
        {
            if (!IsHeader(inline))
            {
                return false;
            }

            // If the next element is null, it means there is no next element.
            Inline? nextInline = inline.NextInline;
            if (nextInline == null)
            {
                return false;
            }

            if (!IsBody(nextInline))
            {
                return false;
            }

            return true;
        }

        private static bool IsExpectedSection(Inline inline, DocumentSection section)
        {
            if (!IsExpectedHeader(inline, section.Header))
            {
                return false;
            }

            // If the next element is null, it means there is no next element.
            Inline? nextInline = inline.NextInline?.NextInline;
            if (nextInline == null)
            {
                return false;
            }

            if (!IsExpectedBody(nextInline, section))
            {
                return false;
            }

            return true;
        }

        private static bool IsHeader(Inline inline)
        {
            Run? run = inline as Run;
            if (run == null)
            {
                return false;
            }

            return (run.FontWeight == FontWeights.Bold) &&
                   (run.TextDecorations == TextDecorations.Underline) &&
                   (run.NextInline is LineBreak);
        }

        private static bool IsExpectedHeader(Inline inline, string expectedText)
        {
            if (!IsHeader(inline))
            {
                return true;
            }

            Run run = (Run)inline;
            return string.Equals(run.Text, expectedText, StringComparison.Ordinal);
        }

        private static bool IsBody(Inline inline)
        {
            Run? run = inline as Run;
            if (run == null)
            {
                return false;
            }

            return (run.FontWeight == FontWeights.Normal) &&
                   (run.TextDecorations.Count == 0) &&
                   (run.NextInline is LineBreak);
        }

        private static bool IsExpectedBody(Inline inline, DocumentSection section)
        {
            if (!IsBody(inline))
            {
                return false;
            }

            Run run = (Run)inline;
          
            if (section.HasBodyFontFamily)
            {
                if (!string.Equals(run.FontFamily.Source, section.BodyFontFamily, StringComparison.Ordinal))
                {
                    return false;
                }
            }

            if (section.HasBodyFontSize)
            {
                if (run.FontSize != section.BodyFontSize!.Value)
                {
                    return false;
                }
            }

            return string.Equals(run.Text, section.Body, StringComparison.Ordinal);
        }
    }
}
