using AzureBlobsAccessor.Utils;
using SimpleBlobUtility.Dtos;
using SimpleBlobUtility.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public App? App =>  Application.Current as App;

        public MainWindow()
        {
            InitializeComponent();

            var containers = BlobUtility.GetContainers(out string errs);
            if (string.IsNullOrWhiteSpace(errs))
            {
                cmbContainers.ItemsSource = containers;
            }
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
            if (this.cmbContainers.SelectedIndex == -1)
            {
                MessageBox.Show("No container item chosen or no containers available (create some in Azure Portal?)");
                return;
            }

            string? containerName = this.cmbContainers.SelectedItem as string;
            if (containerName != null)
            {
                var listFiles = BlobUtility.ListFiles(containerName);
                dgFilesList.ItemsSource = await listFiles;
                this._lastUsedContainer = containerName;
            }
        }



        private async void btnDownloadSelectedFile_Click(object sender, RoutedEventArgs e)
        {
            var result = GetSelectedFileAndContainerName();

            if (result.exit)
            {
                return;
            }
            var loading = FileUtils.AttemptDownloadFile(result.fileName, result.containerName);
            await loading;
        }

        private async void dgFilesList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnDownloadSelectedFile_Click(new object(), new RoutedEventArgs());
        }

        private async void cmbContainers_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            await ListContainerFiles();
        }

        private async void btnViewFile_Click(object sender, RoutedEventArgs e)
        {
            var result = GetSelectedFileAndContainerName();

            if (result.exit)
            {
                if (!string.IsNullOrWhiteSpace(result.errorMsg))
                {
                    MessageBox.Show(result.errorMsg);
                }
                return;
            }

            var result2 = await FileUtils.AttemptDownloadFileToTempFolder(result.fileName, result.containerName);

            if (!result2.success)
            {
                MessageBox.Show(result2.moreInfo);
                return;
            }
            else if (string.IsNullOrEmpty(result2.downloadedFilePath))
            {
                MessageBox.Show("Could not get temp file path");
                return;
            }
            else
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = result2.downloadedFilePath;
                startInfo.UseShellExecute = true; // Let the OS handle opening with default app

                Process.Start(startInfo);
            }
        }

        private void btnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_lastUsedContainer))
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

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = GetSelectedFileAndContainerName();

            if (result.exit)
            {
                if (!string.IsNullOrWhiteSpace(result.errorMsg))
                {
                    MessageBox.Show(result.errorMsg);
                }
                return;
            }

            var results = await BlobUtility.DeleteBlobFile(result.containerName, result.fileName);
            if (results.success)
            {
                MessageBox.Show("File deleted successfully");
                await ListContainerFiles();
            }
            else
            {
                MessageBox.Show($"Error occurred with deleting: {results.errorInfo}");
            }
        }

        private (bool exit, string errorMsg, string fileName, string containerName) GetSelectedFileAndContainerName()
        {
            bool exit = false;
            string fileName = "";
            string errorMsg = "";
            string containerName = "";

            if (dgFilesList.SelectedIndex == -1)
            {
                errorMsg = ("Could not get current grid row, is grid empty?");
                exit = true;
            }

            if (dgFilesList.SelectedCells[0].Item == null)
            {
                errorMsg =("Could not get current grid row, is grid empty?");
                exit = true;
            }

            var flid = dgFilesList.SelectedCells[0].Item as FileListItemDto;
            if (flid == null)
            {
                errorMsg = ("Grid row source does not appear to be a valid Cloud Blob object");
                exit = true;
            }
            else
            {
                fileName = flid.FileName;
                containerName = flid.Container;
                if (string.IsNullOrWhiteSpace(containerName))
                {
                    errorMsg = ("Could not get the container name from file item.");
                    exit = true;
                }
            }
            return (exit, errorMsg, fileName, containerName);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var app = App;
            if (app != null)
            {
                app.Cleanup();
            }
        }
    }
}
