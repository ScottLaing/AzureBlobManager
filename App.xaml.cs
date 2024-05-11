using AzureBlobManager.Utils;
using SimpleBlobUtility.Utils;
using System;
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

        public string EncryptionKey { get; set; } = "";

        public string EncryptionSalt { get; set; } = "";

        private bool _cleanedup = false;

        public bool ConnKeyIsEncrypted = true;

        protected override void OnStartup(StartupEventArgs e)
        {
            GetEncryptionKeys();
            InitBlobConnString();
            base.OnStartup(e);
        }

        private void GetEncryptionKeys()
        {
            var encryptionKey = RegUtils.GetValueFromRegistry(RegNameEncryptionKey);
            var encryptionSalt = RegUtils.GetValueFromRegistry(RegSaltEncryptionKey);

            // if there is no encryption key, then create one and save it to the registry.
            // similar, if there is no salt, then create one and save it to the registry.
            if (string.IsNullOrWhiteSpace(encryptionKey))
            {
                var newGuid = Guid.NewGuid();
                EncryptionKey = newGuid.ToString();
                RegUtils.SaveValueToRegistry(RegNameEncryptionKey, EncryptionKey); // save newly generated key to registry for future
            }
            else
            {
                EncryptionKey = encryptionKey;
            }

            if (string.IsNullOrWhiteSpace(encryptionSalt))
            {
                var newGuid = Guid.NewGuid();
                EncryptionSalt = newGuid.ToString();
                RegUtils.SaveValueToRegistry(RegSaltEncryptionKey, EncryptionSalt); // save newly generated salt to registry for future
            }
            else
            {
                EncryptionSalt = encryptionSalt;
            }
        }

        private void InitBlobConnString()
        {
            BlobUtility.InitializeBlobConnStringFromEnvVariable(); // try to initialize the connection string, from environment variables first
                                                                   // if not found, then check the registry for any saved connection settings.

            if (string.IsNullOrWhiteSpace(BlobUtility.BlobConnectionString))
            {
                GetBlobConnStringFromRegistry();
            }
        }

        private void GetBlobConnStringFromRegistry()
        {
            var result = RegUtils.GetValueFromRegistry(RegNameBlobConnectionKey);
            if (!string.IsNullOrWhiteSpace(result))
            {
                if (ConnKeyIsEncrypted)
                {
                    result = CryptUtils.DecryptString2(result, EncryptionKey, EncryptionSalt);
                }
                BlobUtility.BlobConnectionString = result;
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
