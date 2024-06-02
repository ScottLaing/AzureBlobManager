using AzureBlobManager.Services;
using AzureBlobManager.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Serilog.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using static AzureBlobManager.Constants.UIMessages;

namespace AzureBlobManager.Windows
{
    /// <summary>
    /// Interaction logic for UploadFileWindow.xaml
    /// </summary>
    public partial class UploadFileWindow : Window
    {
        private string _currentContainer;

        private Logger logger = Logging.CreateLogger();
        public IBlobService BlobService => App.Services.GetService<IBlobService>() ?? throw new Exception("could not get blob service DI object");

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

            var result = Task.Run(() => BlobService.SaveFileAsync(fileName, txtVal, _currentContainer)).Result;
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
            OpenFileDialog dlg = UiUtils.SetupDialog();
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                txtFilePath.Text = dlg.FileName;
            }
        }

        /// <summary>
        /// Handles the event when the user double-clicks the window.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Window_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Add your code here to handle the event when the user double-clicks the window
            logger.Debug("Window_MouseDoubleClick call");

            UiUtils.ShowWindowSize(this);
        }

        /// <summary>
        /// Closes the LogViewerWindow.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
