using Microsoft.Win32;
using Serilog.Core;
using System;
using System.Drawing.Imaging;
using System.Windows;
using static AzureBlobManager.Constants;

namespace AzureBlobManager.Utils
{
    public class UiUtils
    {
        private static Logger logger = Logging.CreateLogger();

        public static void ShowWindowSize(Window window)
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
        public static bool ShowConfirmationMessageBox()
        {
            logger.Debug("ShowConfirmationMessageBox call");
            MessageBoxResult result = MessageBox.Show(AreYouSure, Confirmation, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Sets up the file dialog with the appropriate filter settings.
        /// </summary>
        /// <returns>The configured file dialog.</returns>
        public static OpenFileDialog SetupDialog()
        {
            logger.Debug("SetupDialog call");
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = String.Empty;

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
                filter += String.Format(FileDialogMsgs.FileDialogFilterString, FileDialogMsgs.Sep, codecName, codec.FilenameExtension?.ToLower() ?? String.Empty);
            }

            dlg.Filter = filter;
            dlg.FilterIndex = 2;
            return dlg;
        }
    }
}
