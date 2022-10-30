using CommonGeneratorCode;
using EasyToUseGenerator.Resources;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;

namespace EasyToUseGenerator
{
    public partial class HelpWindow : StandardWindow
    {
        public HelpWindow()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        public string GeneratorNameAndVersion
        {
            get
            {
                Assembly? entryAssembly = Assembly.GetEntryAssembly();

                // GetEntryAssembly() will not return null because it is called from managed code
                AssemblyName currentExecutable = entryAssembly!.GetName();

                if (currentExecutable.Version == null)
                {
                    // This error is not expected because this program sets the version on its assembly.
                    throw new Exception(CommonErrorMessages.ThisCaseShouldNeverOccur);
                }

                return string.Format(
                    UserInterface.GeneratorNameAndVersionFormatString, 
                    currentExecutable.Version.Major, 
                    currentExecutable.Version.Minor, 
                    currentExecutable.Version.Build);
            }
        }
        
        private void OnDocumentationClicked(object sender, RoutedEventArgs e)
        {
            LaunchWebBrowser("https://github.com/BenjaminEllett/Generator/blob/main/Documentation/WindowsAppGeneratorUsage.md#generator-windows-app-documentation");
        }

        private void OnPasswordAdviceClicked(object sender, RoutedEventArgs e)
        {
            LaunchWebBrowser("https://github.com/BenjaminEllett/Generator/blob/main/Documentation/PasswordSecurityAdvice.md#password-security-advice");
        }

        private void OnReportProblemClicked(object sender, RoutedEventArgs e)
        {
            LaunchWebBrowser("https://github.com/BenjaminEllett/Generator/issues");
        }

        private void OnDownloadReleaseClicked(object sender, RoutedEventArgs e)
        {
            LaunchWebBrowser("https://github.com/BenjaminEllett/Generator/releases");
        }

        private void OnProjectWebSiteClicked(object sender, RoutedEventArgs e)
        {
            LaunchWebBrowser("https://github.com/BenjaminEllett/Generator");
        }

        private void OnOKClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LaunchWebBrowser(string webSiteUri)
        {
            var interopHelper = new WindowInteropHelper(this);

            ProcessStartInfo openLinkInDefaultBrowser = new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = webSiteUri,

                ErrorDialog = true,
                ErrorDialogParentHandle = interopHelper.Handle,
            };

            Process.Start(openLinkInDefaultBrowser);
        }
    }
}
