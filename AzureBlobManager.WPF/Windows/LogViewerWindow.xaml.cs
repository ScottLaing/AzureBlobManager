using AzureBlobManager.Interfaces;
using Serilog.Core;
using System.Windows;


namespace AzureBlobManager.Windows
{
    /// <summary>
    /// Interaction logic for LogViewerWindow.xaml
    /// </summary>
    public partial class LogViewerWindow : Window
    {
        /// <summary>
        /// Gets the App instance.
        /// </summary>
        public App? App => Application.Current as App;

        /// <summary>
        /// Logger instance.
        /// </summary>
        private Logger logger = Logging.CreateLogger();

        /// <summary>
        /// FileService reference.
        /// </summary>
        private IFileService FileService;

        /// <summary>
        /// UI service reference.
        /// </summary>
        private IUiService UiService;

        /// <summary>
        /// Initializes a new instance of the LogViewerWindow class.
        /// </summary>
        /// <param name="fileService"></param>
        /// <param name="uiService"></param>
        public LogViewerWindow(IFileService fileService, IUiService uiService)
        {
            InitializeComponent();
            UiService = uiService;
            this.txtLogsInfo.Text = Logging.GetLogsText();
            this.FileService = fileService;
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
            //logger.Debug("Window_MouseDoubleClick call");

            //UiService.ShowWindowSize(this);
        }
    }
}
