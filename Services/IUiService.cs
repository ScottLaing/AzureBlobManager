using Microsoft.Win32;
using Serilog.Core;
using System;
using System.Drawing.Imaging;
using System.Windows;
using static AzureBlobManager.Constants;

namespace AzureBlobManager.Services
{
    public interface IUiService
    {
        /// <summary>
        /// Displays the size of the window.
        /// </summary>
        /// <param name="window">The window to display the size of.</param>
        public void ShowWindowSize(Window window);

        /// <summary>
        /// Shows a message box asking if the user is sure.
        /// </summary>
        /// <returns>True if the user is sure, false otherwise.</returns>
        public bool ShowConfirmationMessageBox(string message);

        /// <summary>
        /// Sets up the file dialog with the appropriate filter settings.
        /// </summary>
        /// <returns>The configured file dialog.</returns>
        public OpenFileDialog SetupDialog();
    }
}
