using AzureBlobManager.Interfaces;
using AzureBlobManager.Services;
using AzureBlobManager.Utils;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using System;
using System.Windows;
using static AzureBlobManager.Constants;


namespace AzureBlobManager.Windows
{
    /// <summary>
    /// Interaction logic for LogViewerWindow.xaml
    /// </summary>
    public partial class MoreInfoWindow : Window
    {
        public App? App => Application.Current as App;
        private Logger logger = Logging.CreateLogger();
        private IUiService UiService => App.Services.GetService<UiService>() ?? throw new Exception(DependencyInjectionError);

        public MoreInfoWindow(string message)
        {
            InitializeComponent();
            txtLogsInfo.Text = message; 
        }

        /// <summary>
        /// Closes the LogViewerWindow.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (this.chkDoNotShowAgain.IsChecked != null)
            {
                UiState.ShowViewBlobPreWarning = ! (this.chkDoNotShowAgain.IsChecked ?? false);
            }
            this.Close();
        }

        /// <summary>
        /// Handles the double-click event on the window to display the window size information.
        /// </summary>
        /// <param name="sender">The window that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Window_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            logger.Debug("Window_MouseDoubleClick call");

            UiService.ShowWindowSize(this);
        }

    }
}
