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
using System.Windows;

namespace EasyToUseGenerator.Windows
{
    public partial class MainWindow : StandardWindow
    {
        private Password currentPassword;

        public MainWindow()
        {
            currentPassword = new Password(
                App.Current.Settings.DefaultPasswordType,
                App.Current.Settings.DefaultPasswordLengthInChars);
            DataContext = this;
            InitializeComponent();
        }

        public string NewlyCreatedPassword => currentPassword.Value;

        public string NewlyCreatedPasswordUIAutomationHelpText
        {
            get
            {
                return currentPassword.PasswordType switch
                {
                    PasswordType.AnyKeyOnAnEnglishKeyboardExceptASpace => UserInterface.NewlyCreatedAllCharacterPasswordHelpText,
                    PasswordType.AlphaNumeric => UserInterface.NewlyCreatedAlphaNumericPasswordHelpText,
                    PasswordType.Numeric => string.Empty,

                    // Passwords with spaces are not supported by this application.
                    PasswordType.AnyKeyOnAnEnglishKeyboard => throw new Exception(CommonErrorMessages.ThisCaseShouldNeverOccur),
                    _ => throw new Exception(CommonErrorMessages.ThisCaseShouldNeverOccur),
                };
            }
        }
        public string PasswordStrength => currentPassword.StrengthDisplayText;
        public string PasswordStrengthDescription => currentPassword.StrengthDescription;

        private void OnCopyToClipBoardPressed(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(currentPassword.Value, TextDataFormat.UnicodeText);
        }
        private void OnPrintPasswordPressed(object sender, RoutedEventArgs e)
        {
            CreateWindowAndShowDialogBox(
                () => new PrintPasswordWindow(currentPassword));
        }

        private void OnCreateNewPasswordClicked(object sender, RoutedEventArgs e)
        {
            bool? userCreatedNewPassword;
            CreateNewPasswordWindow createNewPasswordWindow;
            (userCreatedNewPassword, createNewPasswordWindow) = CreateWindowAndShowDialogBox<CreateNewPasswordWindow>();
            if (userCreatedNewPassword == true)
            {
                currentPassword = new Password(
                    createNewPasswordWindow.NewPasswordType,
                    createNewPasswordWindow.NewPasswordLengthInChars);
                NotifyPropertyChanged(nameof(NewlyCreatedPassword));
                NotifyPropertyChanged(nameof(NewlyCreatedPasswordUIAutomationHelpText));
                NotifyPropertyChanged(nameof(PasswordStrength));
                NotifyPropertyChanged(nameof(PasswordStrengthDescription));
            }
        }

        private void OnHelpClicked(object sender, RoutedEventArgs e)
        {
            CreateWindowAndShowDialogBox<HelpWindow>();
        }

        private (bool?, TWindow) CreateWindowAndShowDialogBox<TWindow>()
            where TWindow : Window, new()
        {
            return CreateWindowAndShowDialogBox(() => new TWindow());
        }

        private (bool?, TWindow) CreateWindowAndShowDialogBox<TWindow>(Func<TWindow> createWindow)
            where TWindow : Window
        {
            TWindow newWindow = createWindow();
            newWindow.Owner = this;
            bool? showDialogResult = newWindow.ShowDialog();
            return (showDialogResult, newWindow);
        }
    }
}
