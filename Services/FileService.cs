using AzureBlobManager.Interfaces;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using static AzureBlobManager.Constants;
using static AzureBlobManager.Constants.UIMessages;

namespace AzureBlobManager.Services
{
    public class FileService : IFileService
    {
        private Random random = new Random();

        // Retrieves the current application instance.
        public App? App => Application.Current as App;

        // Generates a temporary file path for the given filename.
        // Parameters:
        //   filename: The name of the file.
        // Returns:
        //   The generated temporary file path.
        public string GetTempFilePath(string filename)
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
        public long GetRandomLongInt(int min = 10000, int max = 99999)
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
        public string GetFileUsingFileDialog(string fileName)
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
