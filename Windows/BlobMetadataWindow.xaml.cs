using SimpleBlobUtility.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace SimpleBlobUtility.Windows
{
    /// <summary>
    /// Interaction logic for BlobMetadataWindow.xaml
    /// </summary>
    public partial class BlobMetadataWindow : Window
    {
        public List<MetadataDto> SourceCollection = new List<MetadataDto>();
        public bool DialogWasSaved = false;

        public App? App =>  Application.Current as App;

        public BlobMetadataWindow(string fileName, List<MetadataDto> sourceCollection)
        {
            InitializeComponent();

            dgMetadataList.ItemsSource = sourceCollection;
            this.txtBlobName.Text = fileName;

            SourceCollection = sourceCollection;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var blobItemChangeWindow = new BlobItemChangeWindow("New Key", "New Value", false);
            blobItemChangeWindow.ShowDialog();
            if (blobItemChangeWindow.DialogWasSaved)
            {

                string blobItemName = blobItemChangeWindow.BlobItemName.Trim();
                if (string.IsNullOrEmpty(blobItemName))
                {
                    MessageBox.Show("Key name cannot be empty, blob item not added.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (SourceCollection.Any(m => m.KeyName.ToLower() == blobItemName.ToLower()))
                {
                    MessageBox.Show("Key name already exists in metadata items. To edit an existing metadata item, select item then click edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                SourceCollection.Add(new MetadataDto()
                {
                    KeyName = blobItemChangeWindow.BlobItemName,
                    Value = blobItemChangeWindow.BlobItemValue ?? ""
                });

                dgMetadataList.ItemsSource = null;
                dgMetadataList.ItemsSource = SourceCollection;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogWasSaved = true;
            this.DialogResult = true;
            this.Close();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var currItem = dgMetadataList.SelectedItem as MetadataDto;
            if (currItem == null)
            {
                MessageBox.Show("No metadata item selected, please select a metadata item to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var blobItemChangeWindow = new BlobItemChangeWindow(currItem.KeyName, currItem.Value, true);

            blobItemChangeWindow.ShowDialog();
            
            if (blobItemChangeWindow.DialogWasSaved)
            {
                string blobItemName = blobItemChangeWindow.BlobItemName.Trim();
                if (string.IsNullOrEmpty(blobItemName))
                {
                    MessageBox.Show("Key name cannot be empty, blob item not editted.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                currItem.Value = blobItemChangeWindow.BlobItemValue ?? "";
 
                dgMetadataList.ItemsSource = null;
                dgMetadataList.ItemsSource = SourceCollection;
            }
        }
    }
}
