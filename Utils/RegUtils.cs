using Microsoft.Win32;
using SimpleBlobUtility;
using System;

namespace AzureBlobManager.Utils
{
    public class RegUtils
    {
        public static string RegSubKey => $"Software\\{Constants.RegistryCompanyName}\\{Constants.RegistryAppName}";

        public static void SaveValueToRegistry(string keyName, string keyValue)
        {
            // Create the subkey if it doesn't exist
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegSubKey))
            {
                // Set the value with appropriate data type
                key.SetValue(keyName, keyValue, RegistryValueKind.String);
            }
        }

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
