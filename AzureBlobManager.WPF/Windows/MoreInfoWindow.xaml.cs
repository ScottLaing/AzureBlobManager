using AzureBlobManager.Utils;
using Serilog.Core;
using System.Windows;

namespace AzureBlobManager.Windows
{
    /// <summary>
    /// Interaction logic for MoreInfoWindow.xaml
    /// </summary>
    public partial class MoreInfoWindow : Window
    {
        public App? App => Application.Current as App;
        private Logger logger = Logging.CreateLogger();
        public bool WasCanceled { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the MoreInfoWindow class.
        /// </summary>
        public MoreInfoWindow(string message)
        {
            InitializeComponent();
            txtLogsInfo.Text = message;
            btnViewBlob.Focus();
        }

        /// <summary>
        /// Closes the LogViewerWindow.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            WasCanceled = true;
            this.Close();
        }

        /// <summary>
        /// Open file handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (this.chkDoNotShowAgain.IsChecked != null)
            {
                UiState.ShowViewBlobPreWarning = !(this.chkDoNotShowAgain.IsChecked ?? false);
            }
            this.Close();
        }
    }
}
