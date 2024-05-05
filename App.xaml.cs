using SimpleBlobUtility.Utils;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace SimpleBlobUtility
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Dictionary<string, string> currentViewFilesWithTempLocations = new Dictionary<string, string>();

        private bool _cleanedup = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            BlobUtility.InitializeConnectionString(); // initialize the connection string, from environment variables currently.
            base.OnStartup(e);
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
