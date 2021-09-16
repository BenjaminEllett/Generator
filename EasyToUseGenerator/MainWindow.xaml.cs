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

using CommonGeneratorCode;
using System.Windows;

namespace EasyToUseGenerator
{
    public partial class MainWindow : StandardWindow
    {
        private Password currentPassword;

        public MainWindow()
        {
            this.currentPassword = new Password(
                App.Current.Settings.DefaultPasswordType,
                App.Current.Settings.DefaultPasswordLengthInChars);
            this.DataContext = this;
            InitializeComponent();
        }

        public string NewlyCreatedPassword => this.currentPassword.Value;
        public string PasswordStrengthTextDescription => this.currentPassword.StrengthDescription;

        private void OnCopyToClipBoardPressed(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.currentPassword.Value, TextDataFormat.Text); 
        }

        private void OnCreateNewPasswordClicked(object sender, RoutedEventArgs e)
        {
            CreateNewPasswordWindow createNewPasswordWindow = new CreateNewPasswordWindow();
            createNewPasswordWindow.Owner = this;
            bool? userCreatedNewPassword = createNewPasswordWindow.ShowDialog();
            if (userCreatedNewPassword == true)
            {
                this.currentPassword = new Password(
                    createNewPasswordWindow.NewPasswordType,
                    createNewPasswordWindow.NewPasswordLengthInChars);
                this.NotifyPropertyChanged(nameof(this.NewlyCreatedPassword));
                this.NotifyPropertyChanged(nameof(this.PasswordStrengthTextDescription));
            }
        }

        private void OnHelpClicked(object sender, RoutedEventArgs e)
        {
            // TODO - Implement this method
            throw new System.NotImplementedException();
        }
    }
}
