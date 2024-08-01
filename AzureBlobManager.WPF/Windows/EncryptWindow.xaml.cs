using AzureBlobManager.Utils;
using System.Windows;
using static AzureBlobManager.Constants.UIMessages;
using static AzureBlobManager.Constants;
using AzureBlobManager.Services;
using AzureBlobManager.Interfaces;
using System.Windows.Documents;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.IO;

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
            var inputText = this.txtPlainText.Text;
            if (string.IsNullOrWhiteSpace(inputText))
            {
                MessageBox.Show(inputText, PleaseEnterAPlainTextToEncrypt, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string outputText;
            int selIndex = this.cmbPasswordSource.SelectedIndex;
            if (selIndex >= 0)
            {
                var salt = _salts[selIndex];
                var key = _keys[selIndex];
                try
                {
                    outputText = CryptUtils.DecryptString(inputText, salt, key);
                }
                catch (System.Security.Cryptography.CryptographicException cex)
                {
                    MessageBox.Show($"Crypto exception: {cex.Message}.", UIMessages.MyAzureBlobManager, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error with decryption: {ex.Message}.", UIMessages.MyAzureBlobManager, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_debug)
                {
                    Debug.WriteLine($"{key} - {salt}", "Key and Salt", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Select password to use.", PleaseEnterInputText, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.txtCypherText.Text = outputText;
        }

        /// <summary>
        /// Handles the click event of the "Encrypt" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            var inputText = this.txtPlainText.Text;
            if (string.IsNullOrWhiteSpace(inputText))
            {
                MessageBox.Show(inputText, PleaseEnterAPlainTextToEncrypt, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string outputText;
            int selIndex = this.cmbPasswordSource.SelectedIndex;
            if (selIndex >= 0)
            {
                var salt = _salts[selIndex];
                var key = _keys[selIndex];

                outputText = CryptUtils.EncryptString(inputText, salt, key);
                if (_debug)
                {
                    Debug.WriteLine($"{key} - {salt}", "Key and Salt", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Select password to use.", PleaseEnterInputText, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.txtCypherText.Text = outputText;
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
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "abm-keys.txt", // Default file name
                DefaultExt = ".txt", // Default file extension
                Filter = "Text documents (.txt)|*.txt" // Filter files by extension
            };

            // Show save file dialog box
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                string filename = dlg.FileName;

                try
                {
                    var keyString = string.Join("\n", _keys);
                    var salts = string.Join("\n", _salts);
                    var header = string.Format($"[{MyAzureBlobManager} - Key Backup - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}]\n\n");
                    File.WriteAllText(filename, string.Format("{0}[Keys]\n{1}\n[Salts]\n{2}", header, keyString, salts));
                    MessageBox.Show($"Key file written successfully to {filename}!  Remember to back it up somewhere safe.", MyAzureBlobManager);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Trouble writing results to {0}, error was {1}.", filename, ex.Message), MyAzureBlobManager);
                }
            }
        }

        private void btnSample_Click(object sender, RoutedEventArgs e)
        {
            this.txtPlainText.Text = Constants.SampleLargeStrings.SampleSpeech;
        }

        private void btnImportKeys_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(FeatureCreationInProgress, string.Format("{0} - Import Keys", MyAzureBlobManager) , MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
