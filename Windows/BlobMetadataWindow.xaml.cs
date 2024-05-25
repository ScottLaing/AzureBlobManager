using SimpleBlobUtility.Dtos;
using SimpleBlobUtility.Utils;
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
        private string containerName = string.Empty;

        public App? App =>  Application.Current as App;

        public BlobMetadataWindow(string containerName, string fileName, List<MetadataDto> sourceCollection)
        {
            InitializeComponent();

            dgMetadataList.ItemsSource = sourceCollection;
            this.txtBlobName.Text = fileName;

            SourceCollection = sourceCollection;
            this.containerName = containerName;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var blobItemChangeWindow = new BlobItemChangeWindow(false, "New Key", "New Value", false);
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

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogWasSaved = true;
            this.DialogResult = true;

            var setResult = await BlobUtility.SetBlobMetadataAsync(this.containerName, txtBlobName.Text, MetadataDto.toDictionary(SourceCollection));
            if (!string.IsNullOrWhiteSpace(setResult))
            {
                MessageBox.Show(string.Format("Error saving metadata: {0}", setResult), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
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

            bool isSystemSetting = Constants.BlobSystemKeyNames.Contains(currItem.KeyName);
            var blobItemChangeWindow = new BlobItemChangeWindow(isSystemSetting, currItem.KeyName, currItem.Value, true);

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

        private void SaveButtonViewbox_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var currItem = dgMetadataList.SelectedItem as MetadataDto;
            if (currItem == null)
            {
                MessageBox.Show("No metadata item selected, please select a metadata item to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool isSystemSetting = Constants.BlobSystemKeyNames.Contains(currItem.KeyName);
            if (isSystemSetting)
            {
                MessageBox.Show("Cannot delete system metadata items.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newList = SourceCollection.Where(m => m.KeyName != currItem.KeyName).ToList();

            SourceCollection = newList;
            dgMetadataList.ItemsSource = null;
            dgMetadataList.ItemsSource = SourceCollection;
        }
    }
}
