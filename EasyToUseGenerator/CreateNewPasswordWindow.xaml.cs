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
using System;
using System.Windows;

namespace EasyToUseGenerator.Windows
{
    public partial class CreateNewPasswordWindow : StandardWindow
    {
        private PasswordType newPasswordType;
        private int newPasswordLengthInChars;
        private bool shouldUpdateDefaultPasswordCharacteristics;

        public CreateNewPasswordWindow()
        {
            NewPasswordType = App.Current.Settings.DefaultPasswordType;
            NewPasswordLengthInChars = App.Current.Settings.DefaultPasswordLengthInChars;
            ShouldUpdateDefaultPasswordCharacteristics = false;

            DataContext = this;
            InitializeComponent();
        }

        public PasswordType NewPasswordType
        {
            get => newPasswordType;

            private set
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
                if (!Password.IsValidPasswordType(value) || PasswordType.AnyKeyOnAnEnglishKeyboard == value)
                {
                    throw new ArgumentException(CommonErrorMessages.ThisCaseShouldNeverOccur, paramName: nameof(value));
                }

                newPasswordType = value;
                NotifyPropertyChanged(nameof(IsAnyKeyWhichCanBeTypedRadioButtonChecked));
                NotifyPropertyChanged(nameof(IsLettersAndNumbersRadioButtonChecked));
                NotifyPropertyChanged(nameof(IsNumbersRadioButtonChecked));
            }
        }

        public int NewPasswordLengthInChars
        {
            get => newPasswordLengthInChars;

            set
            {
                newPasswordLengthInChars = value;
                NotifyPropertyChanged(nameof(NewPasswordLengthInChars));
            }
        }

        public bool ShouldUpdateDefaultPasswordCharacteristics
        {
            get => shouldUpdateDefaultPasswordCharacteristics;

            set
            {
                shouldUpdateDefaultPasswordCharacteristics = value;
                NotifyPropertyChanged(nameof(ShouldUpdateDefaultPasswordCharacteristics));
            }
        }

        public bool IsAnyKeyWhichCanBeTypedRadioButtonChecked
        {
            get => NewPasswordType == PasswordType.AnyKeyOnAnEnglishKeyboardExceptASpace;
        }

        public bool IsLettersAndNumbersRadioButtonChecked
        {
            get => NewPasswordType == PasswordType.AlphaNumeric;
        }

        public bool IsNumbersRadioButtonChecked
        {
            get => NewPasswordType == PasswordType.Numeric;
        }

        private void OnAnyKeyWhichCanBeTypedRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            NewPasswordType = PasswordType.AnyKeyOnAnEnglishKeyboardExceptASpace;
        }

        private void OnLettersAndNumbersRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            NewPasswordType = PasswordType.AlphaNumeric;
        }

        private void OnNumbersRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            NewPasswordType = PasswordType.Numeric;
        }

        private void OnCreatePasswordClicked(object sender, RoutedEventArgs e)
        {
            if (ShouldUpdateDefaultPasswordCharacteristics)
            {
                App.Current.Settings.DefaultPasswordType = NewPasswordType;
                App.Current.Settings.DefaultPasswordLengthInChars = NewPasswordLengthInChars;
            }

            DialogResult = true;
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
