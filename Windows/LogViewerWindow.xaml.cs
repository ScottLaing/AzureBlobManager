using AzureBlobManager;
using System.Windows;


namespace SimpleBlobUtility.Windows
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
