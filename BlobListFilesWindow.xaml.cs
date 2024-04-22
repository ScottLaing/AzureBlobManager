using Microsoft.Win32;
using SimpleBlobUtility.Dtos;
using SimpleBlobUtility.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleBlobUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class BlobListFilesWindow : Window
    {
        public List<FileListItemDto> SourceCollection = new List<FileListItemDto>();

        public BlobListFilesWindow()
        {
            InitializeComponent();

            var containers = BlobUtility.GetContainers(out string errs);
            if (string.IsNullOrWhiteSpace(errs))
            {
                cmbContainers.ItemsSource = containers;
            }
 
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnListContainers_Click(object sender, RoutedEventArgs e)
        {
            var containers = BlobUtility.GetContainers(out string errs);
        }

        private async void btnListFiles_Click(object sender, RoutedEventArgs e)
        {
            await ListContainerFiles();
        }

        private async Task ListContainerFiles()
        {
            string? containerName = this.cmbContainers.SelectedItem as string;
            if (containerName != null)
            {
                var listFiles = BlobUtility.ListFiles(containerName);
                dgFilesList.ItemsSource = await listFiles;
            }
        }

        private async Task AttemptDownloadFile()
        {
            string? containerName = this.cmbContainers.SelectedItem as string;
            if (containerName == null)
            {
                MessageBox.Show("Please select a container in the drop down");
                return;
            }

            if (dgFilesList.SelectedCells[0].Item == null)
            {
                MessageBox.Show("Could not get current grid row, is grid empty?");
                return;
            }

            var flid = dgFilesList.SelectedCells[0].Item as FileListItemDto;
            if (flid == null)
            {
                MessageBox.Show("Grid row source does not appear to be a valid Cloud Blob object");
                return;
            }

            var fileName = flid.FileName;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "All Files|*.*|Text Files|*.txt|JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|PNG Image|*.png";
            saveFileDialog1.Title = "Save File to Local";
            saveFileDialog1.FileName = fileName;
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                var downloadFile = BlobUtility.DownloadBlobFile(containerName, fileName, saveFileDialog1.FileName);
                var results = await downloadFile;
                if (results.Item1)
                {
                    MessageBox.Show("File downloaded successfully");
                }
                else
                {
                    MessageBox.Show($"Error occurred: {results.Item2}");
                }
            }
        }

        private async void btnDownloadSelectedFile_Click(object sender, RoutedEventArgs e)
        {
            var loading = AttemptDownloadFile();
            await loading;
        }

        private async void dgFilesList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var loading = AttemptDownloadFile();
            await loading;
        }

        private async void cmbContainers_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            await ListContainerFiles();
        }
    }
}
