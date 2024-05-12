using SimpleBlobUtility.Dtos;
using SimpleBlobUtility.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static SimpleBlobUtility.Constants.UIMessages;


namespace SimpleBlobUtility.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class BlobMetadataWindow : Window
    {
        public List<MetadataDto> SourceCollection = new List<MetadataDto>();
        private string _lastUsedContainer = "";

        public App? App =>  Application.Current as App;

        public BlobMetadataWindow(List<MetadataDto> sourceCollection)
        {
            InitializeComponent();

            dgMetadataList.ItemsSource = sourceCollection;
        }

    }
}
