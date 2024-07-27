using AzureBlobManager.Utils;
using System.Windows;
using static AzureBlobManager.Constants.UIMessages;
using static AzureBlobManager.Constants;
using AzureBlobManager.Services;
using AzureBlobManager.Interfaces;
using System.Windows.Documents;
using System.Collections.Generic;

namespace AzureBlobManager.Windows
{
    /// <summary>
    /// Interaction logic for EncryptWindow.xaml
    /// </summary>
    public partial class EncryptWindow : Window
    {
        private IRegService _regService;
        private List<string> _keys;
        private List<string> _salts;
        private bool _debug = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptWindow"/> class.
        /// </summary>
        public EncryptWindow(IRegService regService)
        {
            InitializeComponent();

            cmbPasswordSource.ItemsSource = new string[] { "Saved Password1", "Saved Password2", "Saved Password3", "Saved Password4" };
            _regService = regService;

            List<string> salts;
            var keys = _regService.GetEncryptionKeys(out salts);

            _keys = keys;
            _salts = salts;

            cmbPasswordSource.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the click event of the "Decrypt" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            var cypherText = this.txtCypherText.Text;
            if (string.IsNullOrWhiteSpace(cypherText))
            {
                MessageBox.Show(PleaseEnterACypherTextToDecrypt, Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var decrypted = CryptUtils.DecryptString(cypherText);
            this.txtPlainText.Text = decrypted;
        }

        /// <summary>
        /// Handles the click event of the "Encrypt" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            var plainText = this.txtPlainText.Text;
            if (string.IsNullOrWhiteSpace(plainText))
            {
                MessageBox.Show(plainText, PleaseEnterAPlainTextToEncrypt, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string cypherText;
            int selIndex = this.cmbPasswordSource.SelectedIndex;
            if (selIndex >= 0)
            {
                cypherText = CryptUtils.EncryptString(plainText, _salts[selIndex], _keys[selIndex]);
                if (_debug)
                {
                    MessageBox.Show($"{_salts[selIndex]} - {_keys[selIndex]}", "Salt and Key", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Select password to use (Default or other).", PleaseEnterAPlainTextToEncrypt, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.txtCypherText.Text = cypherText;
        }

        /// <summary>
        /// Handles the click event of the "Clear" button.
        /// </summary>
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.txtCypherText.Text = "";
            this.txtPlainText.Text = "";
        }

        private void btnExportKey_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Feature creation in progress, check back soon.", MyAzureBlobManager + "- Export Key", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnSample_Click(object sender, RoutedEventArgs e)
        {
            this.txtPlainText.Text = Constants.SampleLargeStrings.SampleSpeech;
        }
    }
}
