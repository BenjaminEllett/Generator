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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Documents;
using TestUtil;

namespace EasyToUseGenerator.Tests
{
    [TestClass]
    public class SimpleDocumentServiceTests
    {
        private const string testSectionBody = "0123456789 0123456789 0123456789 0123456789 0123456789";

        private static readonly DocumentSection sectionOne = CreateTestSection(1);
        private static readonly DocumentSection sectionTwo = CreateTestSection(2);

        // Cdws stands for Create Document With Sections.  CreateDocumentWithSections() is the name of a method on the
        // ISimpleDocumentService interface.
        [TestMethod]
        public void CdwsThrowsAnExceptionIfADocumentHasNoSections()
        {
            TestHelper.TestActionWhichShouldThrowAnException<ArgumentException>(
                PassAnEmptyListOfDocumentSectionsToCdws,
                "A document must have at least one section.");
            return;

            static void PassAnEmptyListOfDocumentSectionsToCdws()
            {
                CreateDocumentWithSections(new DocumentSection[0]);
            }
        }

        [TestMethod]
        public void CdwsShouldBeAbleToCreateADocumentWithOneSection()
        {
            DocumentSection onlySection = CreateTestSection("Only Section Header");
            CreateDocumentAndVerifyThatItOnlyContainsThisSection(onlySection);
        }

        [TestMethod]
        public void ASectionShouldBeAbleToHaveADifferentTypeFace()
        {
            DocumentSection sectionWithDifferentBodyTypeFace = new DocumentSection(
                "Section whoes body has a different type face.",
                testSectionBody,
                BodyFontFamily: "Courier New");

            CreateDocumentAndVerifyThatItOnlyContainsThisSection(sectionWithDifferentBodyTypeFace);
        }

        [TestMethod]
        public void ASectionShouldBeAbleToHaveADifferentTypeSize()
        {
            DocumentSection sectionWithDifferentBodyTypeSize = new (
                "Section whoes body has a different type size.",
                testSectionBody,
                BodyFontSize: 30);

            CreateDocumentAndVerifyThatItOnlyContainsThisSection(sectionWithDifferentBodyTypeSize);
        }

        [TestMethod]
        public void ASectionShouldBeAbleToHaveADifferentTypeFaceAndTypeSize()
        {
            DocumentSection sectionWithDifferentBodyTypeFaceAndSize = new (
                "Section whoes body has a different type face and size.",
                testSectionBody,
                BodyFontFamily: "Courier New",
                BodyFontSize: 30);

            CreateDocumentAndVerifyThatItOnlyContainsThisSection(sectionWithDifferentBodyTypeFaceAndSize);
        }

        [TestMethod]
        public void CdwsShouldBeAbleToCreateADocumentWithTwoSections()
        {
            FlowDocument documentWithMultipleSections = CreateDocumentWith2Sections();

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

        [TestMethod]
        public void CdwsShouldBeAbleToCreateADocumentSectionsInTheRightOrder()
        {
            FlowDocument documentWithTwoSections = CreateDocumentWith2Sections();

            Run firstSectionRun = GetFirstSection(documentWithTwoSections);
            Assert.IsTrue(
                IsExpectedSection(firstSectionRun, sectionOne),
                "The first section should be section 1.");

            bool foundSecondSection = TryGetNextSection(firstSectionRun, out Run? secondSectionRun);
            Assert.IsTrue(
                foundSecondSection, 
                "The second section should exist because two sections were passed to ISimpleDocumentService.CreateDocumentWithSections()");
            Assert.IsTrue(
                IsExpectedSection(secondSectionRun!, sectionTwo),
                "The second section should be section 2.");

            Assert.IsFalse(
                TryGetNextSection(secondSectionRun!, out Run? thirdSectionRun),
                "The third section should not exist because the document only has two sections.");
        }



        #region Create Test Documents

        private static DocumentSection CreateTestSection(string header) =>
            new DocumentSection(header, testSectionBody);

        private static DocumentSection CreateTestSection(int headerNumber) =>
            CreateTestSection($"Section Header {headerNumber}");

        private static FlowDocument CreateDocumentWithSections(params DocumentSection[] documentSections)
        {
            ISimpleDocumentService simpleDocumentService = new SimpleDocumentService();
            return simpleDocumentService.CreateDocumentWithSections(documentSections);
        }

        private static FlowDocument CreateDocumentWith2Sections() =>
           CreateDocumentWithSections(sectionOne, sectionTwo);

        #endregion Create Test Documents



        #region Simple Document Section Enumerator

        private static Run GetFirstSection(FlowDocument simpleDocumentWithSections)
        {
            Assert.IsTrue(
                simpleDocumentWithSections.Blocks.Count == 1,
                "The document should have only one root element.  If this changes, this function needs to be revised.");

            IList blocks = (IList)simpleDocumentWithSections.Blocks;

            Paragraph paragraph = (Paragraph)blocks[0]!;
            Inline possibleFirstSectionHeader = paragraph.Inlines.FirstInline;
            Assert.IsTrue(
                IsSection(possibleFirstSectionHeader), 
                "A simple document should only have a list of sections.");
            return (Run)possibleFirstSectionHeader;
        }

        private static bool TryGetNextSection(Run currentSectionHeader, [NotNullWhen(returnValue: true)] out Run? nextSectionHeader)
        {
            nextSectionHeader = default(Run);

            if (!TryGetBodyFromSectionHeader(currentSectionHeader, out Run? currentSectionBody))
            {
                return false;
            }

            Inline? currentInline = currentSectionBody.NextInline;

            // There is no section after the current section if currentInline is null.
            if (currentInline == null)
            {
                return false;
            }

            int lineBreakCount = 0;
            const int ExpectedNumberOfLineBreaksBetweenSections = 4;
            
            while (lineBreakCount < ExpectedNumberOfLineBreaksBetweenSections)
            {
                Assert.IsNotNull(
                    currentInline,
                    "There should always be 4 line breaks between sections.  The last section does not have any line breaks after it.");

                Assert.IsTrue(
                    currentInline is LineBreak,
                    "Only LineBreak objects are allowed between sections.  The reason is they provide space between sections.");

                currentInline = currentInline.NextInline;
                lineBreakCount++;
            }

            Assert.IsTrue(
                IsHeader(currentInline),
                "A section header must follow the 4 line breaks which separate sections.");

            nextSectionHeader = (Run)currentInline;
            return true;
        }

        private static bool TryGetBodyFromSectionHeader(Run sectionHeader, [NotNullWhen(returnValue: true)] out Run? sectionBody)
        {
            sectionBody = null;

            // A header has two parts.  The first part is a Run with the header text.  The second part is a LineBreak element.
            Inline? sectionBodyInline = sectionHeader?.NextInline?.NextInline;
            if (sectionBodyInline == null)
            {
                return false;
            }

            Assert.IsTrue(
                IsBody(sectionBodyInline),
                "A body section should come after a header section.");

            sectionBody = (Run)sectionBodyInline;

            return true;
        }

        #endregion Simple Document Section Enumerator



        #region Verification Functions

        private static void CreateDocumentAndVerifyThatItOnlyContainsThisSection(DocumentSection section)
        {
            FlowDocument flowDocument = CreateDocumentWithSections(section);

            Assert.IsTrue(
                DoesDocumentContainSection(flowDocument, section),
                $"The document should contain the '{section.Header}' section.");
            Assert.IsTrue(
                GetSectionCount(flowDocument) == 1,
                "The document should only contain 1 section.");
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
            return IsSectionInternal(inline, IsHeader, IsBody);
        }

        private static bool IsExpectedSection(Inline inline, DocumentSection section)
        {
            return IsSectionInternal(
                inline,
                isHeader: inline => IsExpectedHeader(inline, section.Header),
                isBody: inline => IsExpectedBody(inline, section));
        }

        private static bool IsSectionInternal(Inline inline, Func<Inline, bool> isHeader, Func<Inline, bool> isBody)
        {
            if (!isHeader(inline))
            {
                return false;
            }

            // A header has two parts.  The first part is a Run with the header text.  The second part is a LineBreak element.
            Inline? nextInline = inline?.NextInline?.NextInline;
            if (nextInline == null)
            {
                return false;
            }

            if (!isBody(nextInline))
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
                   (run.TextDecorations.Count == 0);
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

        #endregion Verification Functions
    }
}
