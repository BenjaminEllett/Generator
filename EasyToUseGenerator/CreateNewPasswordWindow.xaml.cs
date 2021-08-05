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
using System.Windows.Shapes;

namespace EasyToUseGenerator
{
    public partial class CreateNewPasswordWindow : Window, INotifyPropertyChanged
    {
        public int newPasswordLengthInChars;

        public CreateNewPasswordWindow()
        {
            this.NewPasswordType = PasswordType.AnyKeyOnAnEnglishKeyboard;

            // 20 was chosen as the default password length because it is the smallest number of characters which produces an almost
            // unguessable password.
            this.NewPasswordLengthInChars = 20;

            this.DataContext = this;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
            this.NewPasswordType = PasswordType.AnyKeyOnAnEnglishKeyboard;
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
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
