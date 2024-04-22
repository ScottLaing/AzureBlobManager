using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleBlobUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            var blobSaveWindow = new BlobSaveWindow();
            blobSaveWindow.Show();
        }

        private void btnViewFiles_Click(object sender, RoutedEventArgs e)
        {
            var blobListFiles = new BlobListFilesWindow();
            blobListFiles.Show();
        }
    }
}
