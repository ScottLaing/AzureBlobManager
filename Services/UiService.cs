using AzureBlobManager.Interfaces;
using Microsoft.Win32;
using Serilog.Core;
using System;
using System.Drawing.Imaging;
using System.Windows;
using static AzureBlobManager.Constants;

namespace AzureBlobManager.Services
{
    public class UiService : IUiService
    {
        private Logger logger = Logging.CreateLogger();

        /// <summary>
        /// Displays the size of the window.
        /// </summary>
        /// <param name="window">The window to display the size of.</param>
        public void ShowWindowSize(Window window)
        {
            logger.Debug("ShowWindowSize call");
            // Display the window size information
            string sizeInfo = string.Format(WindowSizeInfo, window.Width, window.Height);
            MessageBox.Show(sizeInfo, WindowSize, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Shows a message box asking if the user is sure.
        /// </summary>
        /// <returns>True if the user is sure, false otherwise.</returns>
        public bool ShowConfirmationMessageBox(string message = "")
        {
            logger.Debug("ShowConfirmationMessageBox call");
            var warning = string.Format("{0} {1}", message, AreYouSure).Trim();
            MessageBoxResult result = MessageBox.Show(warning, Confirmation, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Sets up the file dialog with the appropriate filter settings.
        /// </summary>
        /// <returns>The configured file dialog.</returns>
        public OpenFileDialog SetupDialog()
        {
            logger.Debug("SetupDialog call");
            var dlg = new OpenFileDialog();
            dlg.Filter = string.Empty;

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            var filter = string.Empty;
            filter = FileDialogMsgs.TextDocuments; // Filter files by extension
            filter += FileDialogMsgs.SetupDialogAllFilesSettings;

            foreach (var codec in codecs)
            {
                if (string.IsNullOrWhiteSpace(codec.CodecName))
                {
                    continue;
                }
                string codecName = codec.CodecName.Substring(8).Replace(FileDialogMsgs.CodecName, FileDialogMsgs.Files).Trim();
                filter += string.Format(FileDialogMsgs.FileDialogFilterString, FileDialogMsgs.Sep, codecName, codec.FilenameExtension?.ToLower() ?? string.Empty);
            }

            dlg.Filter = filter;
            dlg.FilterIndex = 2;
            return dlg;
        }
    }
}
