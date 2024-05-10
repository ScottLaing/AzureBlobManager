using Microsoft.Win32;
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

        public UploadFileWindow(string currentContainer)
        {
            InitializeComponent();

            _currentContainer = currentContainer;
            lblResult.Content = String.Empty;
        }

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
                MessageBox.Show( FileDoesNotExist );
                return;
            }
            var fileName = Path.GetFileName(txtVal);

            var result = Task.Run(() => BlobUtility.SaveFile(fileName, txtVal, _currentContainer)).Result;
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

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = SetupDialog();
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                txtFilePath.Text = dlg.FileName;
            }
        }

        private static OpenFileDialog SetupDialog()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = String.Empty;

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            var filter = string.Empty;
            filter = TextDocuments; // Filter files by extension
            filter += UIMessages.SetupDialogAllFilesSettings;

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
