using Microsoft.Win32;
using System;
using System.IO;

namespace AzureBlobsAccessor.Utils
{
    public class FileUtils
    {
        private static Random random = new Random();

        public static string GetTempFilePath(string filename)
        {
            string ext = Path.GetExtension(filename);
            string rootFileName = Path.GetFileNameWithoutExtension(filename);
            string tempPath = Path.GetTempPath();
            string finalFilePath = "";
            long runaway = 0;
            while (runaway < long.MaxValue)
            {
                string temp2 = rootFileName + "_" + GetRandomLongInt().ToString() + ext;
                string temp3 = Path.Combine(tempPath, temp2);
                if (!File.Exists(temp3))
                {
                    finalFilePath = temp3;
                    break;
                }
                runaway++;
            }
            return finalFilePath;
        }

        public static long GetRandomLongInt(int min = 10000, int max = 99999)
        {
            // Get random integer 
            long randomLong = random.NextInt64(min, max);
            return randomLong;
        }

        public static string GetFileUsingFileDialog(string fileName)
        {
            string chosenFileName;
            SaveFileDialog saveFileDialog1;
            saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "All Files|*.*|Text Files|*.txt|JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|PNG Image|*.png";
            saveFileDialog1.Title = "Save File to Local";
            saveFileDialog1.FileName = fileName;
            var choice = saveFileDialog1.ShowDialog();
            if (choice == true)
            {
                chosenFileName = saveFileDialog1.FileName;
            }
            else
            {
                chosenFileName = "";
            }
            return chosenFileName;
        }
    }
}
