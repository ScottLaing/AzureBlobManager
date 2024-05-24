using SimpleBlobUtility.Dtos;
using System.Collections.Generic;
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
            SourceCollection.Add(new MetadataDto()
            {
                KeyName = "New Key",
                Value = "New Value"
            });
            dgMetadataList.ItemsSource = null;
            dgMetadataList.ItemsSource = SourceCollection;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogWasSaved = true;
            this.DialogResult = true;
            this.Close();
        }
    }
}
