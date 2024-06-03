using AzureBlobManager.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using static AzureBlobManager.Constants;
using static AzureBlobManager.Constants.UIMessages;

namespace AzureBlobManager.Utils
{
    public class FileUtils
    {
        private static Random random = new Random();

        // Retrieves the current application instance.
        public static App? App => Application.Current as App;

        public static IBlobService BlobService => App.Services.GetRequiredService<IBlobService>();

        // Attempts to download a file from the specified container.
        // Parameters:
        //   fileName: The name of the file to download.
        //   containerName: The name of the container where the file is stored.
        public static async Task AttemptDownloadFile(string fileName, string containerName)
        {
            // Prompt the user to choose a file location to save the downloaded file.
            string chosenFileName = FileUtils.GetFileUsingFileDialog(fileName);

            // If the file name is not an empty string, open it for saving.
            if (!string.IsNullOrWhiteSpace(chosenFileName))
            {
                // Download the file from the container and save it to the chosen file location.
                var downloadFile = BlobService.DownloadBlobFileAsync(containerName, fileName, chosenFileName);
                var results = await downloadFile;
                if (results.success)
                {
                    MessageBox.Show(string.Format(DownloadedSuccessfully, fileName));
                }
                else
                {
                    MessageBox.Show(string.Format(ErrorWithDownloading, fileName, results.errorInfo));
                }
            }
        }

        // Attempts to download a file to the temporary folder.
        // Parameters:
        //   fileName: The name of the file to download.
        //   containerName: The name of the container where the file is stored.
        // Returns:
        //   A tuple containing the success status, additional information, and the downloaded file path.
        public static async Task<(bool success, string moreInfo, string downloadedFilePath)> AttemptDownloadFileToTempFolder(string fileName, string containerName)
        {
            var app = App;
            string tempFilePath;

            if (app == null)
            {
                return (false, AppNotDefined, "") ;
            }
            // Check if the file is already present in the temporary folder.
            if (app.currentViewFilesWithTempLocations.ContainsKey(fileName) && File.Exists(app.currentViewFilesWithTempLocations[fileName]))
            {
                tempFilePath = app.currentViewFilesWithTempLocations[fileName];
                return (true, String.Empty, tempFilePath);
            }
            else
            {
                tempFilePath = FileUtils.GetTempFilePath(fileName);
            }

            // If the file name is not an empty string, open it for saving.
            if (!string.IsNullOrEmpty(tempFilePath))
            {
                // Download the file from the container and save it to the temporary file path.
                var results = await BlobService.DownloadBlobFileAsync(containerName, fileName, tempFilePath);
                if (results.Item1)
                {
                    app.currentViewFilesWithTempLocations[fileName] = tempFilePath;
                    return (true, String.Empty, tempFilePath);
                }
                else
                {
                    return (false, results.Item2, String.Empty);
                }
            }
            else
            {
                return (false, CouldNotGetTempFilePath, String.Empty);
            }
        }

        // Generates a temporary file path for the given filename.
        // Parameters:
        //   filename: The name of the file.
        // Returns:
        //   The generated temporary file path.
        public static string GetTempFilePath(string filename)
        {
            string ext = Path.GetExtension(filename);
            string rootFileName = Path.GetFileNameWithoutExtension(filename);
            string tempPath = Path.GetTempPath();
            string finalFilePath = String.Empty;
            long runaway = 0;
            while (runaway < long.MaxValue)
            {
                string temp2 = rootFileName + "_" + GetRandomLongInt().ToString() + ext;
                string temp3 = Path.Combine(tempPath, temp2);
                if (!File.Exists(temp3))
                {
                    finalFilePath = temp3;
                    break;
                }
                runaway++;
            }
            return finalFilePath;
        }

        // Generates a random long integer within the specified range.
        // Parameters:
        //   min: The minimum value of the random long integer (default: 10000).
        //   max: The maximum value of the random long integer (default: 99999).
        // Returns:
        //   The generated random long integer.
        public static long GetRandomLongInt(int min = 10000, int max = 99999)
        {
            // Get random integer 
            long randomLong = random.NextInt64(min, max);
            return randomLong;
        }

        // Prompts the user to choose a file using the file dialog.
        // Parameters:
        //   fileName: The default file name for the file dialog.
        // Returns:
        //   The chosen file name or an empty string if no file was chosen.
        public static string GetFileUsingFileDialog(string fileName)
        {
            string chosenFileName;
            SaveFileDialog saveFileDialog;
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = FileDialogMsgs.AllFiles;
            saveFileDialog.Title = FileDialogMsgs.SaveFileToLocal;
            saveFileDialog.FileName = fileName;
            var choice = saveFileDialog.ShowDialog();
            if (choice == true)
            {
                chosenFileName = saveFileDialog.FileName;
            }
            else
            {
                chosenFileName = String.Empty;
            }
            return chosenFileName;
        }
    }
}
