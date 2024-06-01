using Serilog.Core;
using System.Windows;
using static SimpleBlobUtility.Constants;

namespace SimpleBlobUtility.Utils
{
    public class UiUtils
    {
        private static Logger logger = Logging.CreateLogger();

        public static void ShowWindowSize(Window window)
        {
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
    }
}
