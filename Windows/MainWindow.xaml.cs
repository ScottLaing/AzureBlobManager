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
        private const string GridRowEmptyError = "Could not get current grid row, is grid empty?";
        private const string GridRowObjectNotValid = "Grid row source does not appear to be a valid Cloud Blob object.";
        private const string FileItemNoContainerName = "Could not get the container name from file item.";
        private const string ContainerNotSelected = "No container item chosen or no containers available (create some in Azure Portal?).";
        private const string NotGetTempFilePathError = "Could not get temp file path.";
        private const string NoContainerSelected = "Please select a container to upload to.";
        private const string FileDeletedSuccess = "File deleted successfully.";
        private const string DeletionError = "Error occurred with deleting: {0}.";
        private const string ErrorGettingFilesList = "Error occurred with obtaining files list: {0}.";
        private const string SomeErrorOccurred = "Some error occurred.";

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

        private async Task ListContainerFiles()
        {
            if (this.cmbContainers.SelectedIndex == -1)
            {
                MessageBox.Show( ContainerNotSelected);
                return;
            }

            string? containerName = this.cmbContainers.SelectedItem as string;
            if (containerName != null)
            {
                var listFilesInfo = await BlobUtility.ListFiles(containerName);
                if (!string.IsNullOrWhiteSpace(listFilesInfo.errors))
                {
                    MessageBox.Show(string.Format(ErrorGettingFilesList, listFilesInfo.errors));
                    return;
                }
                dgFilesList.ItemsSource = listFilesInfo.fileItemsList;
                this._lastUsedContainer = containerName;
            }
        }

        private async void btnDownloadSelectedFile_Click(object sender, RoutedEventArgs e)
        {
            var result = GetSelectedFileAndContainerName();

            if (result.errors)
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

            if (result.errors)
            {
                if (!string.IsNullOrWhiteSpace(result.errorMsg))
                {
                    MessageBox.Show(result.errorMsg);
                }
                return;
            }

            var downloadFileResult = await FileUtils.AttemptDownloadFileToTempFolder(result.fileName, result.containerName);

            if (!downloadFileResult.success)
            {
                MessageBox.Show(downloadFileResult.moreInfo);
                return;
            }
            else if (string.IsNullOrEmpty(downloadFileResult.downloadedFilePath))
            {
                MessageBox.Show( NotGetTempFilePathError);
                return;
            }
            else
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = downloadFileResult.downloadedFilePath;
                startInfo.UseShellExecute = true; // Let the OS handle opening with default app

                Process.Start(startInfo);
            }
        }

        private void btnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_lastUsedContainer))
            {
                MessageBox.Show( NoContainerSelected);
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

            if (result.errors)
            {
                if (!string.IsNullOrWhiteSpace(result.errorMsg))
                {
                    MessageBox.Show(result.errorMsg);
                }
                else
                {
                    MessageBox.Show(SomeErrorOccurred);
                }
                return;
            }

            var deleteResult = await BlobUtility.DeleteBlobFile(result.containerName, result.fileName);
            if (deleteResult.success)
            {
                MessageBox.Show(FileDeletedSuccess);
                await ListContainerFiles();
            }
            else
            {
                MessageBox.Show(string.Format(DeletionError, deleteResult.errorInfo));
            }
        }

        private (bool errors, string errorMsg, string fileName, string containerName) GetSelectedFileAndContainerName()
        {
            bool errors = false;
            string fileName = "";
            string errorMsg = "";
            string containerName = "";

            if (dgFilesList.SelectedIndex == -1 || dgFilesList.SelectedCells.Count == 0 || dgFilesList.SelectedCells[0].Item == null)
            {
                errorMsg = GridRowEmptyError;
                errors = true;
            }

            var flid = dgFilesList.SelectedCells[0].Item as FileListItemDto;
            if (flid == null)
            {
                errorMsg = GridRowObjectNotValid;
                errors = true;
            }
            else
            {
                fileName = flid.FileName;
                containerName = flid.Container;
                if (string.IsNullOrWhiteSpace(containerName))
                {
                    errorMsg = FileItemNoContainerName;
                    errors = true;
                }
            }
            return (errors, errorMsg, fileName, containerName);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var app = App;
            if (app != null)
            {
                app.Cleanup();
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            //var uploadFileWindow = new UploadFileWindow(_lastUsedContainer);
            //var resp = uploadFileWindow.ShowDialog();
        }
    }
}
