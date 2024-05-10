using AzureBlobManager.Utils;
using SimpleBlobUtility.Utils;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using static SimpleBlobUtility.Constants;

namespace SimpleBlobUtility
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // these are temp files for viewing, they can be removed when app closes.
        public Dictionary<string, string> currentViewFilesWithTempLocations = new Dictionary<string, string>();

        private bool _cleanedup = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            InitBlobConnString();

            base.OnStartup(e);
        }

        private static void InitBlobConnString()
        {
            BlobUtility.InitializeBlobConnStringFromEnvVariable(); // try to initialize the connection string, from environment variables first
            // if not found, then check the registry for any saved connection settings.
            if (string.IsNullOrWhiteSpace(BlobUtility.BlobConnectionString))
            {
                var result = RegUtils.GetValueFromRegistry(RegNameBlobConnectionKey);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    BlobUtility.BlobConnectionString = result;
                }
            }
        }

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            Cleanup();
            base.OnSessionEnding(e);
        }

        public void Cleanup()
        {
            if (_cleanedup)
            {
                return;
            }
            foreach (string s in currentViewFilesWithTempLocations.Keys)
            {
                var tempPath = currentViewFilesWithTempLocations[s];
                if (File.Exists(tempPath))
                {
                    try
                    {
                        File.Delete(tempPath);
                    }
                    catch { }
                }
            }
            _cleanedup = true;
        }
    }
}
