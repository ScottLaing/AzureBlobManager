using Serilog;
using Serilog.Core;
using System;
using System.IO;


namespace AzureBlobManager
{

    /// <summary>
    /// Represents the configuration for logging in the AzureBlobManager application.
    /// </summary>
    public class LoggingConfig
    {
        /// <summary>
        /// Creates and configures a logger instance for the AzureBlobManager application.
        /// </summary>
        /// <returns>The configured logger instance.</returns>
        public static Logger CreateLogger()
        {
            // Define the local data path for storing log files
            string localDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AzureBlobManager/logs");

            // Configure the logger
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Set the minimum log level to Debug
                .WriteTo.Console() // Write log events to the console
                .WriteTo.File(Path.Combine(localDataPath, "abm.log"), rollingInterval: RollingInterval.Day) // Write log events to a file with daily rolling
                .CreateLogger();

            return logger;
        }
    }
}
