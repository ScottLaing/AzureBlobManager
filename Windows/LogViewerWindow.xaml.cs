using System.Windows;


namespace AzureBlobManager.Windows
{
    /// <summary>
    /// Interaction logic for LogViewerWindow.xaml
    /// </summary>
    public partial class LogViewerWindow : Window
    {
        public App? App => Application.Current as App;

        public LogViewerWindow()
        {
            InitializeComponent();
            this.txtLogsInfo.Text = Logging.GetLogsText();
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
    }
}
