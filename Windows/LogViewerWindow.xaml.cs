using AzureBlobManager.Services;
using Serilog.Core;
using System.Windows;


namespace AzureBlobManager.Windows
{
    /// <summary>
    /// Interaction logic for LogViewerWindow.xaml
    /// </summary>
    public partial class LogViewerWindow : Window
    {
        public App? App => Application.Current as App;
        private Logger logger = Logging.CreateLogger();
        private IFileService fileService;

        public LogViewerWindow(IFileService fileService)
        {
            InitializeComponent();
            this.txtLogsInfo.Text = Logging.GetLogsText();
            this.fileService = fileService;
        }

        /// <summary>
        /// Closes the LogViewerWindow.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
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

            Utils.UiUtils.ShowWindowSize(this);
        }

    }
}
