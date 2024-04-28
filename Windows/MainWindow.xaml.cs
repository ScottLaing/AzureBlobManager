using AzureBlobsAccessor.Utils;
using Microsoft.Win32;
using SimpleBlobUtility.Dtos;
using SimpleBlobUtility.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleBlobUtility.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<FileListItemDto> SourceCollection = new List<FileListItemDto>();
        private string _lastUsedContainer = "";

        public MainWindow()
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
                this._lastUsedContainer = containerName;
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


        private async Task<(bool success, string moreInfo, string downloadedFilePath)> AttemptDownloadFileToTempFolder()
        {
            string? containerName = this.cmbContainers.SelectedItem as string;
            if (containerName == null)
            {
                return (false, "Please select a container in the drop down.", "");
            }
            if (dgFilesList.SelectedCells[0].Item == null)
            {
                return (false, "Please select a file to view.", "");
            }

            var flid = dgFilesList.SelectedCells[0].Item as FileListItemDto;
            if (flid == null)
            {
                return (false, "File selection does not appear to map to a valid Cloud Blob object", "");
            }

            var fileName = flid.FileName;

            string tempFilePath = FileUtils.GetTempFilePath(fileName);

            // If the file name is not an empty string open it for saving.
            if (! string.IsNullOrEmpty(tempFilePath))
            {
                var results = await BlobUtility.DownloadBlobFile(containerName, fileName, tempFilePath);
                if (results.Item1)
                {
                    return (true, "", tempFilePath);
                }
                else
                {
                    return (false, results.Item2, "");
                }
            }
            return (false, "could not get temp file path", "");
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

        private async void btnViewFile_Click(object sender, RoutedEventArgs e)
        {

            if (_lastUsedContainer == "")
            {
                MessageBox.Show("Unclear of which container to use, cannot view.");
                return;
            }

            if (dgFilesList.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a file to view.");
                return;
            }

            var result = await AttemptDownloadFileToTempFolder();

            if (!result.success)
            {
                MessageBox.Show(result.moreInfo);
                return;
            }
            else if (string.IsNullOrEmpty(result.downloadedFilePath))
            {
                MessageBox.Show("Could not get temp file path");
                return;
            }
            else
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = result.downloadedFilePath;
                startInfo.UseShellExecute = true; // Let the OS handle opening with default app

                Process.Start(startInfo);
            }
        }

        private void btnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            if (_lastUsedContainer == "")
            {
                MessageBox.Show("Please select a container to upload to.");
                return;
            }
            var uploadFileWindow = new UploadFileWindow(_lastUsedContainer);
            var resp = uploadFileWindow.ShowDialog();
        }

        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await ListContainerFiles();
        }

        private void btnRefresh2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UploadButtonViewbox_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
