using AzureBlobManager.Utils;
using Microsoft.Win32;
using SimpleBlobUtility.Utils;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

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
                RegUtils.SaveValueToRegistry("BlobConnection", connString);
            }

            this.Close();
        }

        private void SaveButtonViewbox_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
