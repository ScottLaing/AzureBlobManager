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
    }
}
