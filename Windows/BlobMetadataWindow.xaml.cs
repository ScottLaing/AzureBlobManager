using AzureBlobManager;
using SimpleBlobUtility.Dtos;
using SimpleBlobUtility.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static SimpleBlobUtility.Constants;


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

        public App? App => Application.Current as App;

        /// <summary>
        /// Initializes a new instance of the BlobMetadataWindow class.
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="sourceCollection">The collection of metadata items.</param>
        public BlobMetadataWindow(string containerName, string fileName, List<MetadataDto> sourceCollection)
        {
            InitializeComponent();

            dgMetadataList.ItemsSource = sourceCollection;
            this.txtBlobName.Text = fileName;

            SourceCollection = sourceCollection;
            this.containerName = containerName;
        }

        /// <summary>
        /// Handles the click event of the "Add" button.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Create a new instance of BlobItemChangeWindow
            var blobItemChangeWindow = new BlobItemChangeWindow(false, BlobItemNewKey, BlobItemNewValue, false);
            blobItemChangeWindow.ShowDialog();

            // Check if the dialog was saved
            if (blobItemChangeWindow.DialogWasSaved)
            {
                string blobItemName = blobItemChangeWindow.BlobItemName.Trim();

                // Check if the blob item name is empty
                if (string.IsNullOrEmpty(blobItemName))
                {
                    MessageBox.Show(KeyNameCannotBeEmpty, Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Check if the key name already exists in the metadata items
                if (SourceCollection.Any(m => m.KeyName.ToLower() == blobItemName.ToLower()))
                {
                    MessageBox.Show(KeyNameAlreadyExists, Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Add the new metadata item to the source collection
                SourceCollection.Add(new MetadataDto()
                {
                    KeyName = blobItemChangeWindow.BlobItemName,
                    Value = blobItemChangeWindow.BlobItemValue ?? ""
                });

                // Refresh the data grid
                dgMetadataList.ItemsSource = null;
                dgMetadataList.ItemsSource = SourceCollection;
            }
        }

        /// <summary>
        /// Handles the click event of the "Save" button.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogWasSaved = true;
            this.DialogResult = true;

            // Set the blob metadata asynchronously
            var setResult = await BlobUtility.SetBlobMetadataAsync(this.containerName, txtBlobName.Text, MetadataDto.toDictionary(SourceCollection));

            // Check if there was an error saving the metadata
            if (!string.IsNullOrWhiteSpace(setResult))
            {
                MessageBox.Show(string.Format(TroubleSavingMetadata, setResult), Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Close the window
            this.Close();
        }

        /// <summary>
        /// Handles the click event of the "Edit" button.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var currItem = dgMetadataList.SelectedItem as MetadataDto;

            // Check if no metadata item is selected
            if (currItem == null)
            {
                MessageBox.Show(NoMetadataItemSelected, Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool isSystemSetting = Constants.BlobSystemKeyNames.Contains(currItem.KeyName);

            // Create a new instance of BlobItemChangeWindow
            var blobItemChangeWindow = new BlobItemChangeWindow(isSystemSetting, currItem.KeyName, currItem.Value, true);

            blobItemChangeWindow.ShowDialog();

            // Check if the dialog was saved
            if (blobItemChangeWindow.DialogWasSaved)
            {
                string blobItemName = blobItemChangeWindow.BlobItemName.Trim();

                // Check if the blob item name is empty
                if (string.IsNullOrEmpty(blobItemName))
                {
                    MessageBox.Show(KeyNameCannotBeEmptyEdited, Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Update the value of the current metadata item
                currItem.Value = blobItemChangeWindow.BlobItemValue ?? "";

                // Refresh the data grid
                dgMetadataList.ItemsSource = null;
                dgMetadataList.ItemsSource = SourceCollection;
            }
        }


        /// <summary>
        /// Handles the click event of the "Delete" button.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var currItem = dgMetadataList.SelectedItem as MetadataDto;

            // Check if no metadata item is selected
            if (currItem == null)
            {
                MessageBox.Show(NoMetadataItemSelected, Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool isSystemSetting = Constants.BlobSystemKeyNames.Contains(currItem.KeyName);

            // Check if the metadata item is a system setting
            if (isSystemSetting)
            {
                MessageBox.Show(CannotDeleteSystemMetadataItems, Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Remove the metadata item from the source collection
            var newList = SourceCollection.Where(m => m.KeyName != currItem.KeyName).ToList();
            SourceCollection = newList;

            // Refresh the data grid
            dgMetadataList.ItemsSource = null;
            dgMetadataList.ItemsSource = SourceCollection;
        }
    }
}
