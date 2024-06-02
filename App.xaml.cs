using AzureBlobManager.Services;
using AzureBlobManager.Utils;
using AzureBlobManager.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

        public static IHost? AppHost { get; private set; }

        public static IServiceProvider Services => AppHost?.Services ?? throw new Exception("dependency injection setup error");

        private IBlobService BlobService { get; set; } = null!;

        /// <summary>
        /// Event handler for the application startup event.
        /// </summary>
        /// <param name="e">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            logger.Information(ApplicationStartup);

            AppHost = Host.CreateDefaultBuilder(e.Args)
               .ConfigureServices((_, services) => ConfigureMyServices(services))
               .Build();

            var blobServices = Services.GetRequiredService<IBlobService>();
            if (blobServices == null)
            {
                MessageBox.Show("could not get blob service dependency injection class. critical error.");
                Environment.Exit(1);
            }
            BlobService = blobServices;

            GetEncryptionKeys();
            InitBlobConnString();

            // Initialize main window
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

        }


        /// <summary>
        /// Configures the services for dependency injection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance to configure.</param>
        private void ConfigureMyServices(IServiceCollection services)
        {
            services.AddSingleton<IFileService, FileService>(); // Register the FileService as a singleton
            services.AddSingleton<IBlobService, BlobService>(); // Register the BlobService as a singleton
            services.AddSingleton<MainWindow>(); // Register the MainWindow as a singleton
            services.AddTransient<LogViewerWindow>(); // Register the LogViewerWindow as a transient
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
            if (BlobService == null)
            {
                throw new Exception("dependency injection error getting blob service");
            }
            BlobService.InitializeBlobConnStringFromEnvVariable();


            if (string.IsNullOrWhiteSpace(BlobService.BlobConnectionString))
            {
                GetBlobConnStringFromRegistry();
                logger.Information(string.Format(AttemptingToGetConnectionString, BlobService.BlobConnectionString));
            }
            else
            {
                logger.Information(string.Format(UsingConnectionStringFromRegistry, BlobService.BlobConnectionString));
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

                BlobService.BlobConnectionString = result;
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
