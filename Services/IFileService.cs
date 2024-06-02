using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using static AzureBlobManager.Constants;
using static AzureBlobManager.Constants.UIMessages;

namespace AzureBlobManager.Services
{
    public interface IFileService
    {
        public string GetTempFilePath(string filename);

        public long GetRandomLongInt(int min, int max);

        public string GetFileUsingFileDialog(string fileName);
  
    }
}
