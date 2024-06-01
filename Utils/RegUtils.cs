using Microsoft.Win32;
using System;

namespace AzureBlobManager.Utils
{
    public class RegUtils
    {
        public static string RegSubKey => $"Software\\{Constants.RegistryCompanyName}\\{Constants.RegistryAppName}";

        /// <summary>
        /// Saves a value to the Windows Registry under the specified key name.
        /// </summary>
        /// <param name="keyName">The name of the registry key.</param>
        /// <param name="keyValue">The value to be saved.</param>
        public static void SaveValueToRegistry(string keyName, string keyValue)
        {
            // Create the subkey if it doesn't exist
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegSubKey))
            {
                // Set the value with appropriate data type
                key.SetValue(keyName, keyValue, RegistryValueKind.String);
            }
        }

        /// <summary>
        /// Retrieves the value from the Windows Registry under the specified key name.
        /// </summary>
        /// <param name="keyName">The name of the registry key.</param>
        /// <returns>The value retrieved from the registry, or an empty string if the key does not exist.</returns>
        public static string? GetValueFromRegistry(string keyName)
        {
            string? valueAsString = string.Empty;

            try
            {
                RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegSubKey);

                if (key != null)
                {
                    if (key.GetValue(keyName) != null)
                    {
                        valueAsString = (string?)key.GetValue(keyName);
                    }
                    key.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return valueAsString;
        }
    }
}
