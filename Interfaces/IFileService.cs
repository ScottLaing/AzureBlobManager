using AzureBlobManager.Services;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using static AzureBlobManager.Constants;
using static AzureBlobManager.Constants.UIMessages;

namespace AzureBlobManager.Interfaces
{
    public interface IFileService
    {
        /// <summary>
        /// Attempts to download a file from the specified container.
        /// </summary>
        /// <param name="fileName">The name of the file to download.</param>
        /// <param name="containerName">The name of the container where the file is located.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task AttemptDownloadFile(string fileName, string containerName);

        /// <summary>
        /// Attempts to download a file from the specified container to the temporary folder.
        /// </summary>
        /// <param name="fileName">The name of the file to download.</param>
        /// <param name="containerName">The name of the container where the file is located.</param>
        /// <returns>A tuple containing a boolean indicating the success of the operation, a string with additional information, and the path of the downloaded file.</returns>
        public Task<(bool success, string moreInfo, string downloadedFilePath)> AttemptDownloadFileToTempFolder(string fileName, string containerName);

        /// <summary>
        /// Gets the path of the temporary file with the specified filename.
        /// </summary>
        /// <param name="filename">The name of the file.</param>
        /// <returns>The path of the temporary file.</returns>
        public string GetTempFilePath(string filename);

        /// <summary>
        /// Generates a random long integer within the specified range.
        /// </summary>
        /// <param name="min">The minimum value of the range.</param>
        /// <param name="max">The maximum value of the range.</param>
        /// <returns>A random long integer.</returns>
        public long GetRandomLongInt(int min, int max);

        /// <summary>
        /// Opens a file dialog to select a file.
        /// </summary>
        /// <param name="fileName">The name of the file to select.</param>
        /// <returns>The selected file path.</returns>
        public string GetFileUsingFileDialog(string fileName);
    }
}
