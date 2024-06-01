using AzureBlobManager.Dtos;
using AzureBlobManager.Utils;
using Serilog.Core;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static AzureBlobManager.Constants;
using static AzureBlobManager.Constants.UIMessages;

namespace AzureBlobManager.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<FileListItemDto> SourceCollection = new List<FileListItemDto>();
        private string _lastUsedContainer = "";

        public App? App => Application.Current as App;

        private Logger logger = Logging.CreateLogger();

        public MainWindow()
        {
            logger.Information(OpeningMainWindow);
            InitializeComponent();

            RefreshContainersListDropDown();
        }

        /// <summary>
        /// Refreshes the containers list drop-down with the available containers.
        /// </summary>
        private async void RefreshContainersListDropDown()
        {
            logger.Debug("Refreshing containers list drop-down.");
            var containers = BlobUtility.GetContainers(out string errs);
            if (string.IsNullOrWhiteSpace(errs))
            {
                cmbContainers.ItemsSource = containers;
                if (this.cmbContainers.Items.Count > 0)
                {
                    this.cmbContainers.SelectedIndex = 0;
                    await ListContainerFiles();
                }
            }
            else
            {
                MessageBox.Show(TroubleGettingContainers);
                ResetToEmptyDefaults();
            }
        }

        /// <summary>
        /// Resets the drop-downs and data grid to empty defaults.
        /// </summary>
        private void ResetToEmptyDefaults()
        {
            logger.Debug("ResetToEmptyDefaults call");
            cmbContainers.ItemsSource = new List<string>();
            dgFilesList.ItemsSource = new List<FileListItemDto>();
            _lastUsedContainer = "";
        }

        /// <summary>
        /// Lists the files in the selected container and populates the data grid.
        /// </summary>
        private async Task ListContainerFiles()
        {
            logger.Debug("ListContainerFiles call");
            if (this.cmbContainers.SelectedIndex == -1)
            {
                MessageBox.Show(ContainerNotSelected);
                return;
            }

            string? containerName = this.cmbContainers.SelectedItem as string;
            if (containerName != null)
            {
                var listFilesInfo = await BlobUtility.GetContainersFileListAsync(containerName);
                if (!string.IsNullOrWhiteSpace(listFilesInfo.errors))
                {
                    MessageBox.Show(string.Format(ErrorGettingFilesList, listFilesInfo.errors));
                    return;
                }
                dgFilesList.ItemsSource = listFilesInfo.fileItemsList;
                this._lastUsedContainer = containerName;
            }
        }

        /// <summary>
        /// Downloads the selected file from the selected container.
        /// </summary>
        /// <param name="sender">The button that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void btnDownloadSelectedFile_Click(object sender, RoutedEventArgs e)
        {
            logger.Debug("btnDownloadSelectedFile_Click call");
            await DownloadSelFile();
        }

        /// <summary>
        /// Handles the double-click event on the data grid to trigger the file download.
        /// </summary>
        /// <param name="sender">The data grid that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void dgFilesList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            logger.Debug("dgFilesList_MouseDoubleClick call");
            await DownloadSelFile();
        }

        /// <summary>
        /// Downloads the selected file from the selected container.
        /// </summary>
        private async Task DownloadSelFile()
        {
            logger.Debug("DownloadSelFile call");
            var result = GetSelectedFileAndContainerName();

            if (result.errors)
            {
                return;
            }
            var loading = FileUtils.AttemptDownloadFile(result.fileName, result.containerName);
            await loading;
        }

        /// <summary>
        /// Handles the selection change event of the containers drop-down to list the files in the selected container.
        /// </summary>
        /// <param name="sender">The containers drop-down that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void cmbContainers_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            logger.Debug("cmbContainers_SelectionChanged call");
            await ListContainerFiles();
        }

        /// <summary>
        /// Opens the selected file in the default application for viewing.
        /// </summary>
        /// <param name="sender">The button that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void btnViewFile_Click(object sender, RoutedEventArgs e)
        {
            logger.Debug("btnViewFile_Click call");
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
                MessageBox.Show(NotGetTempFilePathError);
                return;
            }
            else
            {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = downloadFileResult.downloadedFilePath;
                startInfo.UseShellExecute = true; // Let the OS handle opening with default app

                Process.Start(startInfo);
            }
        }

        /// <summary>
        /// Opens the upload file window to upload a file to the selected container.
        /// </summary>
        /// <param name="sender">The button that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void btnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            logger.Debug("btnUploadFile_Click call");
            if (string.IsNullOrWhiteSpace(_lastUsedContainer))
            {
                MessageBox.Show(NoContainerSelected);
                return;
            }
            var uploadFileWindow = new UploadFileWindow(_lastUsedContainer);
            var resp = uploadFileWindow.ShowDialog();
            await ListContainerFiles();
        }

        /// <summary>
        /// Refreshes the files in the selected container.
        /// </summary>
        /// <param name="sender">The button that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            logger.Debug("btnRefresh_Click call");
            await ListContainerFiles();
        }

        /// <summary>
        /// Deletes the selected file from the selected container.
        /// </summary>
        /// <param name="sender">The button that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            logger.Debug("btnDelete_Click call");
            if (!UiUtils.ShowConfirmationMessageBox())
            {
                return;
            }

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


            var deleteResult = await BlobUtility.DeleteBlobFileAsync(result.containerName, result.fileName);
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

        /// <summary>
        /// Retrieves the selected file name and container name from the data grid.
        /// </summary>
        /// <returns>A tuple containing the error flag, error message, file name, and container name.</returns>
        private (bool errors, string errorMsg, string fileName, string containerName) GetSelectedFileAndContainerName()
        {
            logger.Debug("GetSelectedFileAndContainerName call");
            bool errors = false;
            string fileName = "";
            string errorMsg = "";
            string containerName = "";

            if (dgFilesList.SelectedIndex == -1 || dgFilesList.SelectedCells.Count == 0 || dgFilesList.SelectedCells[0].Item == null)
            {
                errorMsg = GridRowEmptyError;
                errors = true;
                return (errors, errorMsg, fileName, containerName);
            }

            var flid = dgFilesList.SelectedCells[0].Item as FileListItemDto;
            if (flid == null)
            {
                errorMsg = GridRowObjectNotValid;
                errors = true;
                return (errors, errorMsg, fileName, containerName);
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

        /// <summary>
        /// Cleans up the application resources when the window is closing.
        /// </summary>
        /// <param name="sender">The window that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            logger.Debug("Window_Closing call");
            var app = App;
            if (app != null)
            {
                app.Cleanup();
            }
        }

        /// <summary>
        /// Opens the settings window.
        /// </summary>
        /// <param name="sender">The button that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            logger.Debug("btnSettings_Click call");
            var settingsWindow = new SettingsWindow();
            var resp = settingsWindow.ShowDialog();
            RefreshContainersListDropDown();
        }

        /// <summary>
        /// Opens the blob metadata window for the selected file.
        /// </summary>
        /// <param name="sender">The button that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void btnEditBlobMetadata_Click(object sender, RoutedEventArgs e)
        {
            logger.Debug("btnEditBlobMetadata_Click call");
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

            var metadata = await BlobUtility.GetBlobMetadataAsync(result.containerName, result.fileName);
            if (!string.IsNullOrWhiteSpace(metadata.errors))
            {
                MessageBox.Show(string.Format(MetadataError, result.fileName, metadata.errors));
                return;
            }

            var blobMetadataWindow = new BlobMetadataWindow(result.containerName, result.fileName, MetadataDto.fromDictionary(metadata.metaData));
            blobMetadataWindow.ShowDialog();

            if (blobMetadataWindow.DialogWasSaved)
            {
                var modifiedMetadata = blobMetadataWindow.SourceCollection;
                if (modifiedMetadata == null)
                {
                    MessageBox.Show(AttemptingToUpdateMetadataButValueIsNull);
                    return;
                }
                var modifiedAsDictionary = MetadataDto.toDictionary(modifiedMetadata);
                string errorInUpdating = await BlobUtility.SetBlobMetadataAsync(result.containerName, result.fileName, modifiedAsDictionary);
                if (!string.IsNullOrWhiteSpace(errorInUpdating))
                {
                    MessageBox.Show(string.Format(ErrorWithUpdatingMetadata, errorInUpdating));
                }
            }
        }

        /// <summary>
        /// Handles the double-click event on the window to display the window size information.
        /// </summary>
        /// <param name="sender">The window that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Window_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            logger.Debug("Window_MouseDoubleClick call");
            
            UiUtils.ShowWindowSize(this);
        }


        /// <summary>
        /// Handles the click event of the logs button to open the log viewer window.
        /// </summary>
        /// <param name="sender">The button that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnLogs_Click(object sender, RoutedEventArgs e)
        {
            var logViewerWindow = new LogViewerWindow();
            logViewerWindow.ShowDialog();
        }
    }
}
