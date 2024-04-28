using System.Collections.Generic;
using System.Windows;

namespace SimpleBlobUtility
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Dictionary<string, string> currentViewFilesWithTempLocations = new Dictionary<string, string>();
    }
}
