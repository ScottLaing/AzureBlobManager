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

        public Task AttemptDownloadFile(string fileName, string containerName);

        public Task<(bool success, string moreInfo, string downloadedFilePath)> AttemptDownloadFileToTempFolder(string fileName, string containerName);

        public string GetTempFilePath(string filename);

        public long GetRandomLongInt(int min, int max);

        public string GetFileUsingFileDialog(string fileName);

    }
}
