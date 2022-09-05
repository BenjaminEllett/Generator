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
using System.Windows;
using System.Windows.Documents;
using TestUtil;

namespace EasyToUseGeneratorTests
{
    [TestClass]
    public class SimpleDocumentServiceTests
    {
        private DocumentSection onlySection = new DocumentSection(
                Header: "HeaderOne",
                Body: "0123456789 0123456789 0123456789 0123456789 0123456789");


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
            DocumentSection onlySection = new DocumentSection(
                Header: "HeaderOne",
                Body: "0123456789 0123456789 0123456789 0123456789 0123456789");

            List<DocumentSection> documentSections = new()
            {
                onlySection,
            };

            FlowDocument flowDocument = CreateDocumentWithMultipleSections(documentSections);

            Assert.IsTrue(
                DoesDocumentContainSection(flowDocument, onlySection),
                "The document should contain the only section.");
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
                "The document should contain the only section.");
        }


        private static FlowDocument CreateDocumentWithMultipleSections(IEnumerable<DocumentSection> documentSections)
        {
            ISimpleDocumentService simpleDocumentService = new SimpleDocumentService();
            return simpleDocumentService.CreateDocumentWithMultipleSections(documentSections);
        }

        private static bool DoesDocumentContainSection(FlowDocument flowDocument, DocumentSection section)
        {
            Assert.IsTrue(
                flowDocument.Blocks.Count == 1,
                "The document should have only one root element.  If this changes, this function needs to be revised.");

            IList blocks = (IList)flowDocument.Blocks;
            Paragraph rootElement = (Paragraph)blocks[0]!;

            // Inlines are stored in presentation order.
            foreach (Inline currentInline in rootElement.Inlines)
            {
                if (IsExpectedSection(currentInline, section))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsExpectedSection(Inline inline, DocumentSection section)
        {
            if (!IsExpectedHeader(inline, section.Header))
            {
                return false;
            }

            // If the next element is null, it means there is no next element.
            Inline? nextInline = inline.NextInline;
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

        private static bool IsExpectedHeader(Inline inline, string expectedText)
        {
            Run? run = inline as Run;
            if (run == null)
            {
                return false;
            }

            // PreviousInline returns "null if there is no previous Inline element."  
            // The first header does not have anything before it.
            bool isFirstHeader = inline.PreviousInline != null;
            if (!isFirstHeader)
            {
                int previousInlineCount = 0;
                const int expectedNumberOfLineBreaksBeforeHeader = 3;
                Inline? previousInline = inline.PreviousInline;

                while (previousInlineCount < expectedNumberOfLineBreaksBeforeHeader)
                {
                    // null means there is no previous inline
                    if (previousInline == null)
                    {
                        return false;
                    }

                    if (inline is not LineBreak)
                    {
                        return false;
                    }

                    previousInline = previousInline.PreviousInline;
                    previousInlineCount++;
                }
            }

            return string.Equals(run.Text, expectedText, StringComparison.Ordinal) &&
                   (run.FontWeight == FontWeights.Bold) &&
                   (run.TextDecorations == TextDecorations.Underline);
        }

        private static bool IsExpectedBody(Inline inline, DocumentSection section)
        {
            Run? run = inline as Run;
            if (run == null)
            {
                return false;
            }

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

            return string.Equals(run.Text, section.Body, StringComparison.Ordinal) &&
                   (run.FontWeight == FontWeights.Normal) &&
                   (run.TextDecorations.Count == 0);
        }
    }
}
