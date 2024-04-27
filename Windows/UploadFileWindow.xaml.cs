using SimpleBlobUtility.Utils;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleBlobUtility.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UploadFileWindow : Window
    {
        private string _currentContainer;
        public UploadFileWindow(string currentContainer)
        {
            InitializeComponent();

            _currentContainer = currentContainer;
            lblResult.Content = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var txtVal = this.txtFilePath.Text;

            if (string.IsNullOrWhiteSpace(txtVal))
            {
                MessageBox.Show("Please select a file to upload.");
                return;
            }

            txtVal = txtVal.Trim();

            if (!File.Exists(txtVal))
            {
                MessageBox.Show("File does not appear to exist. Please retry with a valid file name.");
                return;
            }
            var fileName = Path.GetFileName(txtVal);

            var result = Task.Run(() => BlobUtility.SaveFile(fileName, txtVal, _currentContainer)).Result;
            if (!result.Item1)
            {
                this.lblResult.Content = $"Trouble saving file to blob, {result.Item2}";
            }
            else
            {
                this.lblResult.Content = $"{fileName} uploaded successfully, close dialog if done.";
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "";

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = string.Empty;

            var filter = string.Empty;
            filter = "Text documents (*.txt)|*.txt|"; // Filter files by extension
            filter += String.Format("{0} ({1})|{1}", "All Files", "*.*");
            sep = "|";

            foreach (var codec in codecs)
            {
                if (string.IsNullOrWhiteSpace(codec.CodecName))
                {
                    continue;
                }
                string codecName = codec.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                filter += String.Format("{0}{1} ({2})|{2}", sep, codecName, codec.FilenameExtension?.ToLower() ?? "");
                sep = "|";
            }

            dlg.Filter = filter;
            dlg.FilterIndex = 2;
            bool ?result = dlg.ShowDialog();

            if (result == true)
            {
                txtFilePath.Text = dlg.FileName; 
            }
        }

        private void btnListContainers_Click(object sender, RoutedEventArgs e)
        {
            var containers = BlobUtility.GetContainers(out string errs);
        }
    }
}
