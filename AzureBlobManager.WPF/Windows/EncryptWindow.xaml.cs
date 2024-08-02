using AzureBlobManager.Interfaces;
using AzureBlobManager.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using static AzureBlobManager.Constants;
using static AzureBlobManager.Constants.UIMessages;

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

            cmbPasswordSource.ItemsSource = SavedPasswordNames;
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
            var inputText = this.txtInputText.Text;
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
                    MessageBox.Show(String.Format(CryptoException, cex.Message), UIMessages.MyAzureBlobManager, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format(ErrorWithDecryption, ex.Message), UIMessages.MyAzureBlobManager, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_debug)
                {
                    Debug.WriteLine(String.Format(Misc, key, salt), KeyAndSalt, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show(SelectPasswordToUse, PleaseEnterInputText, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.txtOutputText.Text = outputText;
        }

        /// <summary>
        /// Handles the click event of the "Encrypt" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            var inputText = this.txtInputText.Text;
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
                    Debug.WriteLine($"{key} - {salt}", KeyAndSalt, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show(SelectPasswordToUse, PleaseEnterInputText, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.txtOutputText.Text = outputText;
        }

        /// <summary>
        /// Handles the click event of the "Clear" button.
        /// </summary>
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.txtOutputText.Text = String.Empty;
            this.txtInputText.Text = String.Empty;
        }

        private void btnExportKey_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "keys-backup.txt", // Default file name
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
                    StringBuilder sbKeys = new StringBuilder();
                    StringBuilder sbSalts = new StringBuilder();
                    for (int n = 1; n<=4; n++)
                    {
                        sbKeys.AppendLine($"Password{n}:" + _keys[n-1]);
                    }
                    for (int n = 1; n <= 4; n++)
                    {
                        sbSalts.AppendLine($"Salt{n}:" + _salts[n-1]);
                    }

                    var keyString = sbKeys.ToString();
                    var salts = sbSalts.ToString();
                    var header = string.Format(KeyBackup, MyAzureBlobManager, DateTime.Now.ToString(DdMmYyyyHhMmSs));
                    var output = string.Format(KeysSalts, header, keyString, salts);
                    File.WriteAllText(filename, output );
                    if (_debug)
                    {
                        MessageBox.Show(output, MyAzureBlobManager);
                    }
                    MessageBox.Show(String.Format(KeyFileWrittenSuccessfully, filename), MyAzureBlobManager);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(TroubleWritingResults, filename, ex.Message), MyAzureBlobManager);
                }
            }
        }

        private void btnSample_Click(object sender, RoutedEventArgs e)
        {
            this.txtInputText.Text = Constants.SampleLargeStrings.SampleSpeech;
        }

        private void btnImportKeys_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*|Text files (*.txt)";
            openFileDialog.Title = "Select a file";

            if (openFileDialog.ShowDialog() ?? false)
            {
                string filename = openFileDialog.FileName;

                // Do something with the selected file, like read its contents or display its path
                if (_debug)
                {
                    MessageBox.Show("Selected file: " + filename);
                }

                try
                {

                    string[] lines = File.ReadAllLines(filename);
                    // TODO process the new lines, save to internal passwords and salts

                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(TroubleWritingResults, filename, ex.Message), MyAzureBlobManager);
                }
            }
        }
    }
}
