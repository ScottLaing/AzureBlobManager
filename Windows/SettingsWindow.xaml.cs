using AzureBlobManager.Utils;
using SimpleBlobUtility.Utils;
using System;
using System.Windows;
using static SimpleBlobUtility.Constants;

namespace SimpleBlobUtility.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            this.txtAzureConnString.Text = BlobUtility.BlobConnectionString;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var connString = this.txtAzureConnString.Text.Trim();
            BlobUtility.BlobConnectionString = connString;

            if (chkSaveToRegistry.IsChecked == true)
            {
                App? currentApp = Application.Current as App;
                if (currentApp == null)
                {
                    MessageBox.Show("Trouble getting application reference, cannot save to registry.");
                    return;
                }
                string encConnString = CryptUtils.EncryptString2(connString, currentApp.EncryptionKey, currentApp.EncryptionSalt);
                RegUtils.SaveValueToRegistry(RegNameBlobConnectionKey, encConnString);
            }

            this.Close();
        }
    }
}
