﻿using AzureBlobManager.Interfaces;
using AzureBlobManager.Mocks;
using AzureBlobManager.Services;
using AzureBlobManager.Utils;
using AzureBlobManager.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using static AzureBlobManager.Constants;
using static AzureBlobManager.Constants.UIMessages;

namespace AzureBlobManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Encryption key used for encryption and decryption operations.
        public string EncryptionKeyBlob { get; set; } = "";

        // Salt used for encryption and decryption operations.
        public string EncryptionSaltBlob { get; set; } = "";

        // Application startup message.
        public static IHost? AppHost { get; private set; }

        // Dependency injection service provider.
        public static IServiceProvider Services => AppHost?.Services ?? throw new Exception(DependencyInjectionError);

        // Dependency injection service for file operations.
        private IBlobService BlobService { get; set; } = null!;

        // Dependency injection service for registry operations.
        private IRegService RegService { get; set; } = null!;

        // these are temp files for viewing, they can be removed when app closes.
        public Dictionary<string, string> currentViewFilesWithTempLocations = new Dictionary<string, string>();

        // Flag indicating whether the connection key is encrypted.
        public bool ConnKeyIsEncrypted = true;

        // Encryption key used for encryption and decryption operations.
        private bool _cleanedup = false;

        // Logger instance.
        private Logger logger = Logging.CreateLogger();

        /// <summary>
        /// Event handler for the application startup event.
        /// </summary>
        /// <param name="e">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            logger.Information(ApplicationStartup);

            AppHost = Host.CreateDefaultBuilder(e.Args)
               .ConfigureServices((_, services) => ConfigureMyServices(services))
               .Build();

            SetupServices();

            GetEncryptionKeys();
            await InitBlobConnString();
            RegService.CreateInitialEncryptionKeys();

            // work in progress below - on actions to help users to first time setup their azure connection string
            if (string.IsNullOrEmpty(BlobService.BlobConnectionString))
            {
                MessageBox.Show(MissingBlobConnString, MyAzureBlobManager);
            }

            // Initialize main window
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

        }

        /// <summary>
        /// Sets up the services for dependency injection.
        /// </summary>
        private void SetupServices()
        {
            var blobServices = Services.GetRequiredService<IBlobService>();
            if (blobServices == null)
            {
                MessageBox.Show(DependencyInjectionError, MyAzureBlobManager);
                Environment.Exit(1);
            }
            BlobService = blobServices;

            var regService = Services.GetRequiredService<IRegService>();
            if (regService == null)
            {
                MessageBox.Show(DependencyInjectionError, MyAzureBlobManager);
                Environment.Exit(1);
            }
            RegService = regService;
        }


        /// <summary>
        /// Configures the services for dependency injection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance to configure.</param>
        private void ConfigureMyServices(IServiceCollection services)
        {
            services.AddSingleton<IFileService, FileService>(); // Register the FileService as a singleton
            services.AddSingleton<IBlobService, BlobService>(); // Register the BlobService as a singleton
            services.AddSingleton<IRegService, RegService>(); // Register the BlobService as a singleton
            services.AddSingleton<IUiService, UiService>(); // Register the UiService as a singleton
            services.AddSingleton<IBlobServiceClientFactory, SimpleBlobServiceClientFactory>(); // Register the SimpleBlobServiceClientFactory as a singleton
            services.AddSingleton<MainWindow>(); // Register the MainWindow as a singleton
            services.AddTransient<LogViewerWindow>(); // Register the LogViewerWindow as a transient
            services.AddTransient<EncryptWindow>();
        }


        /// <summary>
        /// Retrieves the encryption keys from the registry or generates new ones if they are not found.
        /// </summary>
        private void GetEncryptionKeys()
        {
            var encryptionKeyBlob = RegService.GetValueFromRegistry(RegNameEncryptionKeyZebra);
            var encryptionSaltBlob = RegService.GetValueFromRegistry(RegSaltEncryptionKeyZebra);

            // if there is no encryption key, then create one and save it to the registry.
            // similar, if there is no salt, then create one and save it to the registry.
            if (string.IsNullOrWhiteSpace(encryptionKeyBlob))
            {
                var newGuid = Guid.NewGuid();
                EncryptionKeyBlob = newGuid.ToString();
                RegService.SaveValueToRegistry(RegNameEncryptionKeyZebra, EncryptionKeyBlob); // save newly generated key to registry for future
                logger.Debug(string.Format(EncryptionKeyNotFound, EncryptionKeyBlob));
            }
            else
            {
                logger.Debug(string.Format(EncryptionKeyFound, encryptionKeyBlob));
                EncryptionKeyBlob = encryptionKeyBlob;
            }

            if (string.IsNullOrWhiteSpace(encryptionSaltBlob))
            {
                var newGuid = Guid.NewGuid();
                EncryptionSaltBlob = newGuid.ToString();
                RegService.SaveValueToRegistry(RegSaltEncryptionKeyZebra, EncryptionSaltBlob); // save newly generated salt to registry for future
                logger.Debug(string.Format(EncryptionSaltNotFound, EncryptionSaltBlob));
            }
            else
            {
                logger.Debug(string.Format(EncryptionKeySaltFound, encryptionSaltBlob));
                EncryptionSaltBlob = encryptionSaltBlob;
            }
        }

        /// <summary>
        /// Initializes the Blob connection string from environment variables or registry.
        /// </summary>
        private async Task InitBlobConnString()
        {
            if (BlobService == null)
            {
                throw new Exception(DependencyInjectionError);
            }
            BlobService.InitializeBlobConnStringFromEnvVariable();


            if (string.IsNullOrWhiteSpace(BlobService.BlobConnectionString))
            {
                await GetBlobConnStringFromRegistry();
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
        private async Task  GetBlobConnStringFromRegistry()
        {
            var result = RegService.GetValueFromRegistry(RegNameBlobConnectionKey);
            if (!string.IsNullOrWhiteSpace(result))
            {
                if (ConnKeyIsEncrypted)
                {
                    result = await CryptUtils.DecryptStringAsync(result, EncryptionKeyBlob, EncryptionSaltBlob);
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
