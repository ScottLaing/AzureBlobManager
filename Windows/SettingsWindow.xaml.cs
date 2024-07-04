using AzureBlobManager.Interfaces;
using AzureBlobManager.Utils;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using System;
using System.Windows;
using static AzureBlobManager.Constants;
using static AzureBlobManager.Constants.UIMessages;

namespace AzureBlobManager.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        /// <summary>
        /// BlobService reference.
        /// </summary>
        public IBlobService BlobService => App.Services.GetService<IBlobService>() ?? throw new Exception(DependencyInjectionError);

        /// <summary>
        /// RegService reference.
        /// </summary>
        public IRegService RegService => App.Services.GetService<IRegService>() ?? throw new Exception(DependencyInjectionError);

        /// <summary>
        /// Logger instance.
        /// </summary>
        private Logger logger = Logging.CreateLogger();

        /// <summary>
        /// UI service reference.
        /// </summary>
        private IUiService UiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindow"/> class.
        /// </summary>
        public SettingsWindow(IUiService? uiService = null)
        {
            logger.Debug(StartingSettingsWindow);
            InitializeComponent();
            if (uiService == null)
            {
                this.UiService = App.Services.GetService<IUiService>() ?? throw new Exception(DependencyInjectionError);
            }
            else
            {
                this.UiService = uiService;
            }
            this.txtAzureConnString.Text = BlobService.BlobConnectionString;
        }

        /// <summary>
        /// Event handler for the Save button click event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            logger.Debug(SavingSettings);

            // Get the connection string from the text box and trim any leading or trailing whitespace
            var connString = this.txtAzureConnString.Text.Trim();

            // Update the BlobConnectionString property in the BlobUtility class
            BlobService.BlobConnectionString = connString;

            // Check if the "Save to Registry" checkbox is checked
            if (chkSaveToRegistry.IsChecked == true)
            {
                // Get the current application reference
                App? currentApp = Application.Current as App;

                // Check if the currentApp is null
                if (currentApp == null)
                {
                    // Show an error message if the currentApp is null
                    MessageBox.Show(TroubleGettingApplicationReference, MyAzureBlobManager);
                    return;
                }

                // Encrypt the connection string using the encryption key and salt from the currentApp
                string encConnString = CryptUtils.EncryptString2(connString, currentApp.EncryptionKey, currentApp.EncryptionSalt);

                // Save the encrypted connection string to the registry
                RegService.SaveValueToRegistry(RegNameBlobConnectionKey, encConnString);

                logger.Debug(SavedSettingsToRegistry);
            }

            // Close the SettingsWindow
            this.Close();
        }

        /// <summary>
        /// Event handler for the Window MouseDoubleClick event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Window_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            logger.Debug(WindowMouseDoubleClickCall);
            UiService.ShowWindowSize(this);
        }
    }
}
