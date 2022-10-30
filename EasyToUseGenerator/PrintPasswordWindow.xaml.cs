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

using CommonGeneratorCode;
using EasyToUseGenerator.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace EasyToUseGenerator
{
    public partial class PrintPasswordWindow : StandardWindow
    {
        private readonly Password password;

        public PrintPasswordWindow(Password password)
        {
            this.password = password;
            OptionalWhereIsThisPasswordUsed = string.Empty;
            OptionalAccountUserName = string.Empty;

            DataContext = this;
            InitializeComponent();
        }

        public string OptionalWhereIsThisPasswordUsed { get; set; }
        public string OptionalAccountUserName { get; set; }

        private void OnPrintPasswordClicked(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (true != printDialog.ShowDialog())
            {
                // The user pressed the cancel button in the Print dialog box
                return;
            }

            FlowDocument printedPasswordDocument = CreatePrintedPasswordDocument();

            string printedPasswordPrintQueueDescription =
                string.Format(
                    PrintedPasswordPage.PrintedPasswordPrintQueueDescriptionFormatString,
                    password.LocalTimeInstantCreated.ToString());

            IDocumentPaginatorSource documentPaginatorSource = printedPasswordDocument;
            printDialog.PrintDocument(documentPaginatorSource.DocumentPaginator, printedPasswordPrintQueueDescription);

            Close();
        }

        private FlowDocument CreatePrintedPasswordDocument()
        {
            // Courier New is a default Windows font.  For more information, please see
            // https://docs.microsoft.com/en-us/typography/fonts/windows_10_font_list .
            const string DocumentPasswordFontFamily = "Courier New"; // This fount makes it easy to distinguish similar looking characters like 1, l and I.

            List<DocumentSection> documentSections = new();

            AddSectionIfIsUsed(PrintedPasswordPage.WhereIsThisPasswordUsedHeader, OptionalWhereIsThisPasswordUsed);
            AddSectionIfIsUsed(PrintedPasswordPage.AccountUserNameHeader, OptionalAccountUserName);
            AddSectionIfIsUsed(
                PrintedPasswordPage.PrintedPasswordHeader,
                password.Value,
                textFontFamily: DocumentPasswordFontFamily,
                textFontSize: 30); // A large font size is used because a password should be easy to read.

            DateTime instantPasswordCreatedLocalTime = password.UtcInstantCreated.ToLocalTime();
            AddSectionIfIsUsed(PrintedPasswordPage.DatePasswordCreatedHeader, instantPasswordCreatedLocalTime.ToString());

            ISimpleDocumentService simpleDocumentService = App.Current.GetService<ISimpleDocumentService>();
            return simpleDocumentService.CreateDocumentWithMultipleSections(documentSections);



            void AddSectionIfIsUsed(string headerText, string bodyText, string? textFontFamily = null, double? textFontSize = null)
            {
                if (!IsSectionUsed(bodyText))
                {
                    return;
                }

                documentSections.Add(new DocumentSection(headerText, bodyText, textFontFamily, textFontSize));
            }

            static bool IsSectionUsed(string sectionText)
            {
                return !string.IsNullOrWhiteSpace(sectionText);
            }
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
