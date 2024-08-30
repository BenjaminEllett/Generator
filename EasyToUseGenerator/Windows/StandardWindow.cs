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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EasyToUseGenerator
{
    /// <summary>
    /// This class is responsible for the standard look and feel of this application's windows.  Each window should do the following:
    /// 
    /// 1. It should be derivid from this class.
    /// 2. It's style should be the StandardWindowStyle.  Here is how this style is specified in XAML.
    /// 
    ///     Style="{StaticResource StandardWindowStyle}"
    /// 
    /// </summary>
    public class StandardWindow : Window, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TitleBarBackgroundProperty =
            DependencyProperty.Register(
                name: "TitleBarBackground",
                propertyType: typeof(Brush),
                ownerType: typeof(StandardWindow),
                new PropertyMetadata(defaultValue: Brushes.Black));

        public StandardWindow()
        {
            this.Loaded += OnWindowLoaded;            
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public Brush TitleBarBackground
        {
            get => (Brush)this.GetValue(TitleBarBackgroundProperty);
            set => this.SetValue(TitleBarBackgroundProperty, value);
        }

        // The main window should have a minimize button but none of the dialog box (child) windows should.
        public bool HasMinimizeButton => this.Owner == null;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            SetClickedEventHandlerOnButton("MinimizeButton", OnMinimizeButtonClicked);
            SetClickedEventHandlerOnButton("CloseButton", OnClosedButtonClicked);

            this.ShowInTaskbar = (this.Owner == null);
            return;



            void SetClickedEventHandlerOnButton(string buttonName, RoutedEventHandler clickedEventHandler)
            {
                Button button = (Button)this.Template.FindName(buttonName, this);

                // If this line crashes with a NullReferenceException, the exception probably occurs because the window does not have the
                // StandardWindowStyle.
                button.Click += clickedEventHandler;
            }
        }

        private void OnMinimizeButtonClicked(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void OnClosedButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
