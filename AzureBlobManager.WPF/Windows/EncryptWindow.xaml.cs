using AzureBlobManager.Utils;
using System.Windows;
using static AzureBlobManager.Constants.UIMessages;
using static AzureBlobManager.Constants;

namespace AzureBlobManager.Windows
{
    /// <summary>
    /// Interaction logic for EncryptWindow.xaml
    /// </summary>
    public partial class EncryptWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptWindow"/> class.
        /// </summary>
        public EncryptWindow()
        {
            InitializeComponent();

            cmbPasswordSource.ItemsSource = new string[] { "Default", "Saved Password1", "Saved Password2", "Saved Password3" };
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
            if (this.cmbPasswordSource.SelectedIndex == 0)
            {
                cypherText = CryptUtils.EncryptString(plainText);
            }
            else if (this.cmbPasswordSource.SelectedIndex == 1)
            {
                cypherText = CryptUtils.EncryptString(plainText);
            }
            else if (this.cmbPasswordSource.SelectedIndex == 2)
            {
                cypherText = CryptUtils.EncryptString(plainText);
            }
            else if (this.cmbPasswordSource.SelectedIndex == 3)
            {
                cypherText = CryptUtils.EncryptString(plainText);
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
