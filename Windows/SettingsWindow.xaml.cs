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

            RegUtils.SaveValueToRegistry("BlobConnection", connString);

            var result = RegUtils.GetValueFromRegistry("BlobConnection");

            var same = result == connString;

            this.Close();
        }
    }
}
