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
    public partial class CreateNewPasswordWindow : StandardWindow
    {
        public int newPasswordLengthInChars;

        public CreateNewPasswordWindow()
        {
            this.NewPasswordType = PasswordType.AnyKeyOnAnEnglishKeyboardExceptASpace;

            // 20 was chosen as the default password length because it is the smallest number of characters which produces an almost
            // unguessable password.
            this.NewPasswordLengthInChars = 20;

            this.DataContext = this;
            InitializeComponent();
        }

        public PasswordType NewPasswordType { get; private set; }

        public int NewPasswordLengthInChars 
        { 
            get
            {
                return this.newPasswordLengthInChars;
            }

            set
            {
                this.newPasswordLengthInChars = value;
                NotifyPropertyChanged(nameof(this.NewPasswordLengthInChars));
            }
        }

        private void OnAnyKeyWhichCanBeTypedRadioButtonClicked(object sender, RoutedEventArgs e)
        {
            // Spaces are not allowed in passwords for the following reasons:
            //
            // 1. It is hard for users to determine if a spaces is at the start or end of a password
            // 2. It is hard for users to determine how many spaces are between two non-space password characters (i.e. how many spaces
            //    are between l and d in the following password: "fdjkdjfsldkslkjfsl   dfslfdks")
            // 3. Some password algorithms convert multuple spaces into one space. This means a user may think their password is
            //    "sasajd  dsfdsfds" but there password really is "sasajd dsfdsfds".  This leads to problems.
            // 4. Disallowing spaces does not significantly reduce the security of passwords.
            //
            this.NewPasswordType = PasswordType.AnyKeyOnAnEnglishKeyboardExceptASpace;
        }

        private void OnLettersAndNumbersRadioButtonClicked(object sender, RoutedEventArgs e)
        {
            this.NewPasswordType = PasswordType.AlphaNumeric;
        }

        private void OnNumbersRadioButtonClicked(object sender, RoutedEventArgs e)
        {
            this.NewPasswordType = PasswordType.Numeric;
        }

        private void OnCreatePasswordClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
