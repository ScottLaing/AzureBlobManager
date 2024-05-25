using System;
using System.Text.RegularExpressions;
using System.Windows;


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

        private bool isEditting = false;
        private bool isSystemData = false;

        public App? App =>  System.Windows.Application.Current as App;

        public BlobItemChangeWindow(bool isSystemData, string keyName, string keyValue, bool isEditting)
        {
            InitializeComponent();

            this.txtBlobItemName.Text = keyName;
            this.txtBlobItemValue.Text = keyValue;
            this.isEditting = isEditting;
            this.isSystemData = isSystemData;

            // if editting don't allow them to change the keyname
            if (isEditting)
            {
                this.txtBlobItemName.IsReadOnly = true;
                this.txtBlobItemName.Background = System.Windows.Media.Brushes.LightGray;
            }

            if (isSystemData)
            {
                this.txtBlobItemName.IsReadOnly = true;
                this.txtBlobItemName.Background = System.Windows.Media.Brushes.LightGray;
                this.txtBlobItemValue.IsReadOnly = true;
                this.txtBlobItemValue.Background = System.Windows.Media.Brushes.LightGray;
            }
        }

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
                MessageBox.Show("Key name cannot contain whitespace.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!isSystemData)
            {
                if (Constants.BlobSystemKeyNames.Contains(trimmed)) 
                {
                    MessageBox.Show("Key name is a reserved system key name, cannot be used. Please use another keyname.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (trimmed.Contains("$"))
                {
                    MessageBox.Show("Key name contains unallowed characters.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            DialogWasSaved = true;
            this.DialogResult = true;
            BlobItemValue = this.txtBlobItemValue.Text;
            this.Close();
        }
    }
}
