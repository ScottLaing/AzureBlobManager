using AzureBlobManager.Interfaces;
using AzureBlobManager.Utils;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using System;
using System.Windows;
using static AzureBlobManager.Constants;

namespace AzureBlobManager.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class EncryptWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindow"/> class.
        /// </summary>
        public EncryptWindow()
        {

            InitializeComponent();

        }

        private void btnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            var s1 = this.txtCypherText.Text;
            var s2 = CryptUtils.DecryptString(s1);
            this.txtPlainText.Text = s2;
        }

        private void btnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            var s1 = this.txtPlainText.Text;
            var s2 = CryptUtils.EncryptString(s1);
            this.txtCypherText.Text = s2;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.txtCypherText.Text = "";
            this.txtPlainText.Text = "";
        }
    }
}
