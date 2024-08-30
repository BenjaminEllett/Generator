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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace EasyToUseGenerator
{
    public interface ISimpleDocumentService
    {
        public FlowDocument CreateDocumentWithSections(IEnumerable<DocumentSection> documentSections);
    }

    public class SimpleDocumentService : ISimpleDocumentService
    {
        public FlowDocument CreateDocumentWithSections(IEnumerable<DocumentSection> documentSections)
        {
            if (!documentSections.Any())
            {
                throw new ArgumentException(
                    "A document must have at least one section.",
                    paramName: nameof(documentSections));
            }

            // Calibri is a default Windows font.  For more information, please see
            // https://docs.microsoft.com/en-us/typography/fonts/windows_10_font_list .
            const string DocumentFontFamily = "Calibri";

            // Create the flow document
            FlowDocument document = new FlowDocument()
            {
                FontFamily = new FontFamily(DocumentFontFamily),
                FontSize = 20,
                Foreground = Brushes.Black,
                Background = Brushes.White,
            };

            var sections = new Paragraph();
            document.Blocks.Add(sections);

            foreach (DocumentSection currentDocumentSection in documentSections)
            {
                bool isFirstSection = !sections.Inlines.Any();
                if (!isFirstSection)
                {
                    // Add lines after the previous section
                    const int numLinesBetweenSections = 4;

                    for (int lineBreakIndex = 0; lineBreakIndex < numLinesBetweenSections; lineBreakIndex++)
                    {
                        AddLineBreak(sections);
                    }
                }

                // Create section header 
                var sectionHeader = new Run(currentDocumentSection.Header)
                {
                    FontWeight = FontWeights.Bold,
                };

                sectionHeader.TextDecorations = TextDecorations.Underline;

                // Create section body
                var sectionBody = new Run(currentDocumentSection.Body);

                if (currentDocumentSection.HasBodyFontFamily)
                {
                    sectionBody.FontFamily = new FontFamily(currentDocumentSection.BodyFontFamily);
                }

                if (currentDocumentSection.HasBodyFontSize)
                {
                    sectionBody.FontSize = currentDocumentSection.BodyFontSize!.Value;
                }

                // Put new section in the document
                AddText(sections, sectionHeader);
                AddLineBreak(sections);
                AddText(sections, sectionBody);
            }

            return document;



            static void AddText(Paragraph paragraph, Run text)
            {
                paragraph.Inlines.Add(text);
            }

            static void AddLineBreak(Paragraph paragraph)
            {
                paragraph.Inlines.Add(new LineBreak());
            }
        }
    }
}
