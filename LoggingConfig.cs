using Serilog;
using Serilog.Core;
using System;
using System.IO;
using System.Text;


namespace AzureBlobManager
{

    /// <summary>
    /// Represents the configuration for logging in the AzureBlobManager application.
    /// </summary>
    public class Logging
    {
        public const string LogLocation = "AzureBlobManager/logs";
        public const string LogFileName = "abm.log";

        public static string LogFilesPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), LogLocation);
            }
        }

        /// <summary>
        /// Creates and configures a logger instance for the AzureBlobManager application.
        /// </summary>
        /// <returns>The configured logger instance.</returns>
        public static Logger CreateLogger()
        {
            // Configure the logger
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Set the minimum log level to Debug
                .WriteTo.Console() // Write log events to the console
                .WriteTo.File(Path.Combine(LogFilesPath, LogFileName), rollingInterval: RollingInterval.Day) // Write log events to a file with daily rolling
                .CreateLogger();

            return logger;
        }

        public static string GetLogsText()
        {
            StringBuilder output = new StringBuilder();
            string pattern = "*.log"; // Search for all files with .txt extension

            string[] files = Directory.GetFiles(LogFilesPath, pattern);
            foreach (string file in files)
            {
                output.AppendLine(file.Replace("/","\\"));
                output.AppendLine("===============================================");
                try
                {
                    output.AppendLine(File.ReadAllText(file));
                }
                catch (Exception ex)
                {
                    output.AppendLine(string.Format("Error occurred: [{0}] file contents are likely inaccessible (may be current in-use log), try again later. Error details: {1}", file, ex.Message));
                }

                output.AppendLine();
                output.AppendLine();
            }

            return output.ToString();
        }
    }
}
