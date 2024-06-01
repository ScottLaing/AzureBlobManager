using AzureBlobManager.Utils;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using static AzureBlobManager.Constants;

namespace AzureBlobManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // these are temp files for viewing, they can be removed when app closes.
        public Dictionary<string, string> currentViewFilesWithTempLocations = new Dictionary<string, string>();

        // Encryption key used for encryption and decryption operations.
        public string EncryptionKey { get; set; } = "";

        // Salt used for encryption and decryption operations.
        public string EncryptionSalt { get; set; } = "";

        private bool _cleanedup = false;

        // Flag indicating whether the connection key is encrypted.
        public bool ConnKeyIsEncrypted = true;

        private Logger logger = Logging.CreateLogger();

        /// <summary>
        /// Event handler for the application startup event.
        /// </summary>
        /// <param name="e">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            logger.Information(ApplicationStartup);
            GetEncryptionKeys();
            InitBlobConnString();
            base.OnStartup(e);
        }

        /// <summary>
        /// Retrieves the encryption keys from the registry or generates new ones if they are not found.
        /// </summary>
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
                logger.Debug(string.Format(EncryptionKeyNotFound, EncryptionKey));
            }
            else
            {
                logger.Debug(string.Format(EncryptionKeyFound, encryptionKey));
                EncryptionKey = encryptionKey;
            }

            if (string.IsNullOrWhiteSpace(encryptionSalt))
            {
                var newGuid = Guid.NewGuid();
                EncryptionSalt = newGuid.ToString();
                RegUtils.SaveValueToRegistry(RegSaltEncryptionKey, EncryptionSalt); // save newly generated salt to registry for future
                logger.Debug(string.Format(EncryptionSaltNotFound, EncryptionSalt));
            }
            else
            {
                logger.Debug(string.Format(EncryptionKeySaltFound, encryptionSalt));
                EncryptionSalt = encryptionSalt;
            }
        }

        /// <summary>
        /// Initializes the Blob connection string from environment variables or registry.
        /// </summary>
        private void InitBlobConnString()
        {
            BlobUtility.InitializeBlobConnStringFromEnvVariable(); // try to initialize the connection string, from environment variables first
                                                                   // if not found, then check the registry for any saved connection settings.

            if (string.IsNullOrWhiteSpace(BlobUtility.BlobConnectionString))
            {
                GetBlobConnStringFromRegistry();
                logger.Information(string.Format(AttemptingToGetConnectionString, BlobUtility.BlobConnectionString));
            }
            else
            {
                logger.Information(string.Format(UsingConnectionStringFromRegistry, BlobUtility.BlobConnectionString));
            }
        }

        /// <summary>
        /// Retrieves the Blob connection string from the registry and decrypts it if necessary.
        /// </summary>
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

        /// <summary>
        /// Event handler for the session ending event.
        /// </summary>
        /// <param name="e">The <see cref="SessionEndingCancelEventArgs"/> instance containing the event data.</param>
        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            Cleanup();
            base.OnSessionEnding(e);
        }

        /// <summary>
        /// Performs application cleanup by deleting temporary files.
        /// </summary>
        public void Cleanup()
        {
            logger.Information(ApplicationCleanup);
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
