using Microsoft.Win32;
using SimpleBlobUtility.Utils;
using SimpleBlobUtility;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace AzureBlobsAccessor.Utils
{
    public class FileUtils
    {
        private static Random random = new Random();

        public static App? App => Application.Current as App;

        public static async Task AttemptDownloadFile(string fileName, string containerName)
        {
            string chosenFileName = FileUtils.GetFileUsingFileDialog(fileName);

            // If the file name is not an empty string open it for saving.
            if (!string.IsNullOrWhiteSpace(chosenFileName))
            {
                var downloadFile = BlobUtility.DownloadBlobFile(containerName, fileName, chosenFileName);
                var results = await downloadFile;
                if (results.success)
                {
                    MessageBox.Show($"{fileName} downloaded successfully");
                }
                else
                {
                    MessageBox.Show($"Error with downloading {fileName}: {results.errorInfo}");
                }
            }
        }

        public static async Task<(bool success, string moreInfo, string downloadedFilePath)> AttemptDownloadFileToTempFolder(string fileName, string containerName)
        {
            var app = App;
            string tempFilePath;
            if (app.currentViewFilesWithTempLocations.ContainsKey(fileName) && File.Exists(app.currentViewFilesWithTempLocations[fileName]))
            {
                tempFilePath = app.currentViewFilesWithTempLocations[fileName];
                return (true, "", tempFilePath);
            }
            else
            {
                tempFilePath = FileUtils.GetTempFilePath(fileName);
            }

            // If the file name is not an empty string open it for saving.
            if (!string.IsNullOrEmpty(tempFilePath))
            {
                var results = await BlobUtility.DownloadBlobFile(containerName, fileName, tempFilePath);
                if (results.Item1)
                {
                    app.currentViewFilesWithTempLocations[fileName] = tempFilePath;
                    return (true, "", tempFilePath);
                }
                else
                {
                    return (false, results.Item2, "");
                }
            }
            else
            {
                return (false, "could not get temp file path", "");
            }
        }

        public static string GetTempFilePath(string filename)
        {
            string ext = Path.GetExtension(filename);
            string rootFileName = Path.GetFileNameWithoutExtension(filename);
            string tempPath = Path.GetTempPath();
            string finalFilePath = "";
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

        public static long GetRandomLongInt(int min = 10000, int max = 99999)
        {
            // Get random integer 
            long randomLong = random.NextInt64(min, max);
            return randomLong;
        }

        public static string GetFileUsingFileDialog(string fileName)
        {
            string chosenFileName;
            SaveFileDialog saveFileDialog1;
            saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "All Files|*.*|Text Files|*.txt|JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|PNG Image|*.png";
            saveFileDialog1.Title = "Save File to Local";
            saveFileDialog1.FileName = fileName;
            var choice = saveFileDialog1.ShowDialog();
            if (choice == true)
            {
                chosenFileName = saveFileDialog1.FileName;
            }
            else
            {
                chosenFileName = "";
            }
            return chosenFileName;
        }
    }
}
