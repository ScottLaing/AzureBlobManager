using SimpleBlobUtility.Dtos;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;


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

        public App? App =>  Application.Current as App;

        public BlobItemChangeWindow(string keyName, string keyValue, bool isEditting)
        {
            InitializeComponent();

            this.txtBlobItemName.Text = keyName;
            this.txtBlobItemValue.Text = keyValue;
            this.isEditting = isEditting;

            // if editting don't allow them to change the keyname
            if (isEditting)
            {
                this.txtBlobItemName.IsReadOnly = true;
                this.txtBlobItemName.Background = System.Windows.Media.Brushes.LightGray;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogWasSaved = true;
            this.DialogResult = true;

            BlobItemName = this.txtBlobItemName.Text;
            BlobItemValue = this.txtBlobItemValue.Text;
            this.Close();
        }
    }
}
