using AzureBlobManager.Interfaces;
using AzureBlobManager.Services;
using AzureBlobManager.Utils;
using Microsoft.Win32;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private IFileService _fileService;
        private List<string> _keys;
        private List<string> _salts;

        private List<string> _oldKeys = new List<string>();
        private List<string> _oldSalts = new List<string>();

        private bool _debug = true;
        private bool _showMessageBoxes = false;

        private IUiService UiService;

        private Logger logger = Logging.CreateLogger();


        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptWindow"/> class.
        /// </summary>
        /// <param name="fileService"></param>
        /// <param name="regService"></param>
        public EncryptWindow(IRegService regService, IFileService fileService, IUiService uiService)
        {
            InitializeComponent();
            this.UiService = uiService;

            cmbPasswordSource.ItemsSource = SavedPasswordNames;
            _regService = regService;

            _fileService = fileService;

            List<string> salts;
            List<string> keys = _regService.GetEncryptionKeys(out salts);

            _keys = keys;
            _salts = salts;

            cmbPasswordSource.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the click event of the "Decrypt" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            string inputText = this.txtInputText.Text;
            if (string.IsNullOrWhiteSpace(inputText))
            {
                MessageBox.Show(PleaseEnterAPlainTextToEncrypt, UIMessages.MyAzureBlobManager, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string outputText;
            int selIndex = this.cmbPasswordSource.SelectedIndex;
            if (selIndex >= 0)
            {
                string salt = _salts[selIndex];
                string key = _keys[selIndex];
                try
                {
                    outputText = await CryptUtils.DecryptStringAsync(inputText, salt, key);
                }
                catch (System.Security.Cryptography.CryptographicException cex)
                {
                    if (_showMessageBoxes)
                    {
                        MessageBox.Show(String.Format(CryptoException, cex.Message), UIMessages.MyAzureBlobManager, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    this.txtOutputText.Text = DecryptionError;
                    return;
                }
                catch (Exception ex)
                {
                    if (_showMessageBoxes)
                    {
                        MessageBox.Show(String.Format(ErrorWithDecryption, ex.Message), UIMessages.MyAzureBlobManager, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    this.txtOutputText.Text = DecryptionError;
                    return;
                }

                if (_debug)
                {
                    Debug.WriteLine(String.Format(Misc, key, salt));
                }
            }
            else
            {
                MessageBox.Show(SelectPasswordToUse, UIMessages.MyAzureBlobManager, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.txtOutputText.Text = outputText;
        }

        /// <summary>
        /// Handles the click event of the "Encrypt" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            string inputText = this.txtInputText.Text;
            if (string.IsNullOrWhiteSpace(inputText))
            {
                MessageBox.Show(PleaseEnterAPlainTextToEncrypt, UIMessages.MyAzureBlobManager, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string outputText;
            int selIndex = this.cmbPasswordSource.SelectedIndex;
            if (selIndex >= 0)
            {
                string salt = _salts[selIndex];
                string key = _keys[selIndex];

                var encrypingTask = CryptUtils.EncryptStringAsync(inputText, salt, key);
                await encrypingTask;
                outputText = encrypingTask.Result;

                if (_debug)
                {
                    Debug.WriteLine(string.Format($"{key} - {salt}", key, salt));
                }
            }
            else
            {
                MessageBox.Show(SelectPasswordToUse, UIMessages.MyAzureBlobManager, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.txtOutputText.Text = outputText;
        }

        /// <summary>
        /// Handles the click event of the "Clear" button.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="sender"></param>
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.txtOutputText.Text = String.Empty;
            this.txtInputText.Text = String.Empty;
        }

        /// <summary>
        /// Export keys to a text file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportKeys_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = DefaultKeyFileName, // Default file name
                DefaultExt = TextExt, // Default file extension
                Filter = TextDocsFilter // Filter files by extension
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
                        sbKeys.AppendLine(string.Format(Password, n) + _keys[n-1]);
                    }
                    for (int n = 1; n <= 4; n++)
                    {
                        sbSalts.AppendLine(string.Format(SaltDisplay, n) + _salts[n-1]);
                    }

                    var keyString = sbKeys.ToString();
                    var salts = sbSalts.ToString();
                    var header = string.Format(KeyBackup, MyAzureBlobManager, DateTime.Now.ToString(DdMmYyyyHhMmSs));
                    var output = string.Format(KeysSalts, header, keyString, salts);
                    File.WriteAllText(filename, output);
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

        /// <summary>
        /// Create some sample text for testing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSample_Click(object sender, RoutedEventArgs e)
        {
            this.txtInputText.Text = Constants.SampleLargeStrings.SampleSpeech;
        }

        /// <summary>
        /// Import keys from a text file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportKeys_Click(object sender, RoutedEventArgs e)
        {
            var id = MessageBox.Show(ThisWillOverwrite, MyAzureBlobManager, MessageBoxButton.YesNo);
            if (id != MessageBoxResult.Yes)
            {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = AllFilesTextFiles;
            openFileDialog.Title = SelectFile;

            if (openFileDialog.ShowDialog() ?? false)
            {
                string filename = openFileDialog.FileName;

                // Do something with the selected file, like read its contents or display its path
                if (_debug)
                {
                    MessageBox.Show(string.Format(SelectedFile, filename), MyAzureBlobManager);
                }

                // backup current keys and salts for recovery
                var oldSalts = _salts.ToList();
                var oldKeys = _keys.ToList();

                // also save old keys to class level for emergency backup type scenarios,tbd
                _oldKeys = oldKeys;
                _oldSalts = oldSalts;

                try
                {

                    string[] lines = File.ReadAllLines(filename);
                    foreach (string line in lines)
                    {
                        string trimmedLine = line.Trim();
                        if (trimmedLine.StartsWith(PasswordPrefix))
                        {
                            int n = int.Parse(trimmedLine.Substring(8, 1));
                            _keys[n - 1] = trimmedLine.Substring(10);
                        }
                        else if (trimmedLine.StartsWith(SaltPrefix))
                        {
                            int n = int.Parse(trimmedLine.Substring(4, 1));
                            _salts[n - 1] = trimmedLine.Substring(6);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(TroubleReadingKeyFile, filename, ex.Message), MyAzureBlobManager);
                    // restore old keys in case of error
                    _salts = oldSalts.ToList();
                    _keys = oldKeys.ToList();
                }
            }
        }

        /// <summary>
        /// Save output text to a file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnSaveOutput_Click(object sender, RoutedEventArgs e)
        {
            string chosenFileName = _fileService.GetSaveFileUsingFileDialog(DefaultOutputText);

            // If the file name is not an empty string, open it for saving.
            if (!string.IsNullOrWhiteSpace(chosenFileName))
            {
                var output = this.txtOutputText.Text;
                await File.WriteAllTextAsync(chosenFileName, output);
                MessageBox.Show(string.Format(OutputSavedSuccess, chosenFileName), MyAzureBlobManager);
            }
        }

        /// <summary>
        /// Open a file and read into input text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            string chosenFileName = _fileService.GetOpenFileUsingFileDialog(String.Empty);

            // If the file name is not an empty string, open it for saving.
            if (!string.IsNullOrWhiteSpace(chosenFileName))
            {
                var readingTask = File.ReadAllTextAsync(chosenFileName);
                string input = await readingTask;
                this.txtInputText.Text = input;
                this.txtOutputText.Clear();
            }
        }

        /// <summary>
        /// Encrypt a file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnEncryptFile_Click(object sender, RoutedEventArgs e)
        {
            string outputText;
            int selIndex = this.cmbPasswordSource.SelectedIndex;
            if (selIndex >= 0)
            {
                string salt = _salts[selIndex];
                string key = _keys[selIndex];

                if (string.IsNullOrWhiteSpace(salt) || string.IsNullOrWhiteSpace(key))
                {
                    MessageBox.Show(CriticalFileEncryptionError, MyAzureBlobManager);
                    return;
                }

                string chosenFileName = _fileService.GetOpenBinaryFileUsingFileDialog(String.Empty);

                try
                {
                    // If the file name is not an empty string, open it for saving.
                    if (!string.IsNullOrWhiteSpace(chosenFileName))
                    {
                        var readingTask = File.ReadAllBytesAsync(chosenFileName);
                        byte[] fileBytes = await readingTask;

                        var ext = Path.GetExtension(chosenFileName);

                        ext = ext.Replace(Period, String.Empty);
                        ext = ext.Substring(0, Math.Min(ext.Length, PaddingLengthFileSuffix));
                        ext = ext.PadRight(PaddingLengthFileSuffix, EqualsChar);

                        string base64String = Convert.ToBase64String(fileBytes);

                        var encryptingTask = CryptUtils.EncryptStringAsync(ext + base64String, salt, key);
                        await encryptingTask;

                        outputText = encryptingTask.Result;

                        var outputFile = chosenFileName + EncryptTextSuffix;
                        var writingTask = File.WriteAllTextAsync(outputFile, outputText);
                        
                        await writingTask;
                        
                        MessageBox.Show(string.Format(EncryptedFileCreated, outputFile), MyAzureBlobManager);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(ErrorEncountered, ex.Message), MyAzureBlobManager);
                }
            }
            else
            {
                MessageBox.Show(SelectPasswordToUse, MyAzureBlobManager, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        /// <summary>
        /// Decrypt a file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDecryptFile_Click(object sender, RoutedEventArgs e)
        {
            string outputText;
            int selIndex = this.cmbPasswordSource.SelectedIndex;
            if (selIndex >= 0)
            {
                string salt = _salts[selIndex];
                string key = _keys[selIndex];

                if (string.IsNullOrWhiteSpace(salt) || string.IsNullOrWhiteSpace(key))
                {
                    MessageBox.Show(CriticalFileEncryptionError, MyAzureBlobManager);
                    return;
                }

                string chosenFileName = _fileService.GetOpenFileUsingFileDialog(String.Empty);

                try
                {
                    // If the file name is not an empty string, open it for saving.
                    if (!string.IsNullOrWhiteSpace(chosenFileName))
                    {
                        string fileContent = File.ReadAllText(chosenFileName);
                        var decryptingTask = CryptUtils.DecryptStringAsync(fileContent, salt, key);
                        outputText = await decryptingTask;
                        string firstPart = outputText.Substring(0, PaddingLengthFileSuffix);
                        firstPart = firstPart.Replace(EqualsString, String.Empty);

                        byte[] decodedBytes = Convert.FromBase64String(outputText.Substring(10));
                        if (chosenFileName.EndsWith(EncryptTextSuffix))
                        {
                            chosenFileName = chosenFileName.Substring(0, chosenFileName.Length - EncryptTextSuffix.Length);
                        }

                        if (chosenFileName.EndsWith(Period + firstPart))
                        {
                            chosenFileName = chosenFileName.Substring(0, chosenFileName.Length - (firstPart.Length + 1));
                        }

                        string outputFile = chosenFileName + DecryptedSuffix + firstPart;
                        if (decodedBytes != null)
                        {
                            await File.WriteAllBytesAsync(outputFile, decodedBytes);
                        }

                        MessageBox.Show(string.Format(DecryptedFileCreated, outputFile), MyAzureBlobManager);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(ErrorEncountered, ex.Message), MyAzureBlobManager);
                }
            }
            else
            {
                MessageBox.Show(SelectPasswordToUse, UIMessages.MyAzureBlobManager, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        /// <summary>
        /// Handles the double-click event on the window to display the window size information.
        /// </summary>
        /// <param name="sender">The window that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Window_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ShowWindowDoubleClickDebugMessageBox)
            {
                logger.Debug(WindowMouseDoubleClickCall);
                UiService.ShowWindowSize(this);
            }
        }
    }
}
