using AzureBlobManager;
using System.Text.RegularExpressions;
using System.Windows;
using static SimpleBlobUtility.Constants;


namespace SimpleBlobUtility.Windows
{
    /// <summary>
    /// Interaction logic for BlobItemChangeWindow.xaml
    /// </summary>
    public partial class BlobItemChangeWindow : Window
    {

        public bool DialogWasSaved = false;
        public string BlobItemName { get; set; } = string.Empty;
        public string BlobItemValue { get; set; } = string.Empty;

        private bool isSystemData = false;

        public App? App => System.Windows.Application.Current as App;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobItemChangeWindow"/> class.
        /// </summary>
        /// <param name="isSystemData">Indicates whether the data is system data.</param>
        /// <param name="keyName">The key name.</param>
        /// <param name="keyValue">The key value.</param>
        /// <param name="isEditting">Indicates whether the window is in editing mode.</param>
        public BlobItemChangeWindow(bool isSystemData, string keyName, string keyValue, bool isEditting)
        {
            InitializeComponent();

            this.txtBlobItemName.Text = keyName;
            this.txtBlobItemValue.Text = keyValue;
            this.isSystemData = isSystemData;

            // if editing don't allow them to change the keyname
            if (isEditting)
            {
                this.txtBlobItemName.IsReadOnly = true;
                this.txtBlobItemName.Background = System.Windows.Media.Brushes.LightGray;
                txtBlobItemValue.SelectAll();
                this.txtBlobItemValue.Focus();
            }
            else
            {
                txtBlobItemName.SelectAll();
                this.txtBlobItemName.Focus();
            }

            if (isSystemData)
            {
                this.txtBlobItemName.IsReadOnly = true;
                this.txtBlobItemName.Background = System.Windows.Media.Brushes.LightGray;
                this.txtBlobItemValue.IsReadOnly = true;
                this.txtBlobItemValue.Background = System.Windows.Media.Brushes.LightGray;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            BlobItemName = txtBlobItemName.Text;
            string trimmed = BlobItemName.Trim();

            if (isSystemData)
            {
                DialogWasSaved = false;
                DialogResult = false;
                BlobItemValue = this.txtBlobItemValue.Text;
                this.Close();
            }

            bool hasWhitespace = Regex.IsMatch(trimmed, @"\s");
            if (hasWhitespace)
            {
                // Show error message if key name contains whitespace
                MessageBox.Show(KeyNameCannotContainWhitespace, Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!isSystemData)
            {
                if (Constants.BlobSystemKeyNames.Contains(trimmed))
                {
                    // Show error message if key name is reserved
                    MessageBox.Show(KeyNameIsReserved, Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (trimmed.Contains("$"))
                {
                    // Show error message if key name contains unallowed characters
                    MessageBox.Show(KeyNameContainsUnallowedCharacters, Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            DialogWasSaved = true;
            this.DialogResult = true;
            BlobItemValue = this.txtBlobItemValue.Text;
            this.Close();
        }

        /// <summary>
        /// Handles the GotFocus event of the txtBlobItemValue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void txtBlobItemValue_GotFocus(object sender, RoutedEventArgs e)
        {
            // Select all text when the textbox gets focus
            this.txtBlobItemValue.SelectAll();
        }
    }
}
