using AzureBlobManager.Utils;
using System.Windows;

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
                MessageBox.Show("Please enter a cypher text to decrypt.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(plainText, "Please enter a plain text to encrypt.", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var cypherText = CryptUtils.EncryptString(plainText);
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
    }
}
