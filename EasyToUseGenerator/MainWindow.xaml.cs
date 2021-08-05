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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasyToUseGenerator
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const int DefaultPasswordLengthInChars = 20;

        private Password currentPassword;

        public MainWindow()
        {
            this.currentPassword = new Password(
                PasswordType.AnyKeyOnAnEnglishKeyboard, 
                passwordLengthInCharacters: DefaultPasswordLengthInChars);
            this.DataContext = this;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string NewlyCreatedPassword => this.currentPassword.Value;
        public string PasswordStrengthTextDescription => this.currentPassword.StrengthDescription;

        // https://docs.microsoft.com/en-us/windows/win32/winmsg/about-windows#application-windows
        // https://docs.microsoft.com/en-us/windows/win32/gdi/nonclient-area
        //      These links are useful because they explain what the verious parts of a Window are called.
        //
        // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getsystemmetrics
        //      This link is useful because the SystemParameters uses the Win32 GetSystemMetrics() function to get a lot of the data it returns.
        //
        public double SystemSmallIconWidth => SystemParameters.SmallIconWidth;
        public double SystemSmallIconHeight => SystemParameters.SmallIconHeight;

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            SystemParameters.StaticPropertyChanged += OnSystemParametersChanged;
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            SystemParameters.StaticPropertyChanged -= OnSystemParametersChanged;
        }

        private void OnSystemParametersChanged(object sender, PropertyChangedEventArgs e)
        {
            // "An Empty value or null for the propertyName parameter indicates that all of the properties have changed."
            // (https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.propertychangedeventargs.propertyname?view=net-5.0)
            bool allPropertiesChanged = string.IsNullOrEmpty(e.PropertyName);

            NotifyPropertyChangedIfNecessary(nameof(SystemParameters.SmallIconWidth), nameof(this.SystemSmallIconWidth));
            NotifyPropertyChangedIfNecessary(nameof(SystemParameters.SmallIconHeight), nameof(this.SystemSmallIconHeight));
            return;


            void NotifyPropertyChangedIfNecessary(string systemParametersPropertyName, string propertyName)
            {
                if (allPropertiesChanged || string.Equals(e.PropertyName, systemParametersPropertyName, StringComparison.Ordinal))
                {
                    NotifyPropertyChanged(propertyName);
                }
            }
        }

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
                NotifyPropertyChanged(nameof(this.NewlyCreatedPassword));
                NotifyPropertyChanged(nameof(this.PasswordStrengthTextDescription));
            }
        }

        private void OnHelpClicked(object sender, RoutedEventArgs e)
        {

        }

        private void OnMinimizeButtonClicked(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void OnClosedButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
