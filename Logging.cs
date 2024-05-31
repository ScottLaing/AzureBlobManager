using Serilog;
using Serilog.Core;
using System;
using System.IO;
using System.Text;


namespace AzureBlobManager
{
    /// <summary>
    /// Represents the Logging information and configuration for logging in the AzureBlobManager application.
    /// Logging uses Serilog for logging to the console and to a file.
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
            string pattern = "*.log"; 

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
                    output.AppendLine(string.Format("ERROR: [{0}] file contents are likely inaccessible (may be current or recently in-use log), please try again later.", file));
                    output.AppendLine(string.Format("ERROR DETAILS: {0}.", ex.Message));
                }

                output.AppendLine();
                output.AppendLine();
            }

            return output.ToString();
        }
    }
}
