using Serilog;
using Serilog.Core;
using System;
using System.IO;
using System.Text;
using static AzureBlobManager.Constants;


namespace AzureBlobManager
{
    /// <summary>
    /// Represents the Logging information and configuration for logging in the AzureBlobManager application.
    /// Logging uses Serilog for logging to the console and to a file.
    /// </summary>
    public class Logging
    {
        /// <summary>
        /// Gets the path to the log files directory.
        /// </summary>
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
               // .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext} - {Method}:{LineNumber} - {Message}{NewLine}{Exception}")
                .WriteTo.File(Path.Combine(LogFilesPath, LogFileName), rollingInterval: RollingInterval.Day) // Write log events to a file with daily rolling
                .CreateLogger();

            return logger;
        }

        /// <summary>
        /// Replaces forward slashes with backslashes in the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The input string with forward slashes replaced by backslashes.</returns>
        public static string FixSlashes(string input)
        {
            return input.Replace("/", "\\");
        }

        /// <summary>
        /// Retrieves the text content of all log files in the log directory.
        /// </summary>
        /// <returns>The text content of all log files.</returns>
        public static string GetLogsText()
        {
            StringBuilder output = new StringBuilder();

            string[] files = Directory.GetFiles(LogFilesPath, LogPattern);
            foreach (string file in files)
            {
                output.AppendLine(FixSlashes(file));
                output.AppendLine(LogSeparator);
                try
                {
                    output.AppendLine(File.ReadAllText(file));
                }
                catch (Exception ex)
                {
                    output.AppendLine(string.Format(ErrorFileContentsAreLikelyInaccessible, file));
                    output.AppendLine(string.Format(ErrorDetails, ex.Message));
                }

                output.AppendLine();
                output.AppendLine();
            }

            return output.ToString();
        }
    }
}
