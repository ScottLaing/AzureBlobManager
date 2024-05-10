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
                RegUtils.SaveValueToRegistry(RegNameBlobConnectionKey, connString);
            }

            this.Close();
        }
    }
}
