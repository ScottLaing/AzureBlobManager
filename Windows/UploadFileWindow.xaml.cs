using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Serilog.Core;
using SimpleBlobUtility.Utils;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using static SimpleBlobUtility.Constants;
using static SimpleBlobUtility.Constants.UIMessages;

namespace SimpleBlobUtility.Windows
{
    /// <summary>
    /// Interaction logic for UploadFileWindow.xaml
    /// </summary>
    public partial class UploadFileWindow : Window
    {
        private string _currentContainer;

        private Logger logger = Logging.CreateLogger();

        /// Initializes a new instance of the UploadFileWindow class.
        /// </summary>
        /// <param name="currentContainer">The current container.</param>
        public UploadFileWindow(string currentContainer)
        {
            InitializeComponent();

            _currentContainer = currentContainer;
            lblResult.Content = String.Empty;
        }

        /// <summary>
        /// Handles the event when the user clicks the "Upload File" button.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            var txtVal = this.txtFilePath.Text;

            if (string.IsNullOrWhiteSpace(txtVal))
            {
                MessageBox.Show(PleaseSelectFile);
                return;
            }

            txtVal = txtVal.Trim();

            if (!File.Exists(txtVal))
            {
                MessageBox.Show(FileDoesNotExist);
                return;
            }
            var fileName = Path.GetFileName(txtVal);

            var result = Task.Run(() => BlobUtility.SaveFileAsync(fileName, txtVal, _currentContainer)).Result;
            if (!result.Item1)
            {
                string msg = string.Format(TroubleSavingFile, result.Item2);
                this.lblResult.Content = msg;
            }
            else
            {
                this.lblResult.Content = string.Format(FileUploadedSuccessfully, fileName);
            }
        }

        /// <summary>
        /// Handles the event when the user clicks the "Select" button.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = SetupDialog();
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                txtFilePath.Text = dlg.FileName;
            }
        }

        /// <summary>
        /// Sets up the file dialog with the appropriate filter settings.
        /// </summary>
        /// <returns>The configured file dialog.</returns>
        private static OpenFileDialog SetupDialog()
        {
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

        private void Window_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            logger.Debug("Window_MouseDoubleClick call");

            UiUtils.ShowWindowSize(this);
        }
    }
}
