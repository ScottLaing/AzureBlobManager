using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;


namespace AzureBlobManager.Utils
{
    public class RegUtils
    {
        private static string appName = "AzureBlobManager58";  // Replace with your application name

        public static void SaveValueToRegistry(string keyName, string keyValue)
        {

            // Create the subkey if it doesn't exist
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey($"HKEY_CURRENT_USER\\Software\\{appName}"))
            {
                // Set the value with appropriate data type
                key.SetValue(keyName, keyValue, RegistryValueKind.String);
            }
        }

        public static string? GetValueFromRegistry(string keyName)
        {
            string keyPath = @$"HKEY_CURRENT_USER\Software\{appName}";

            object? registryValue = Registry.GetValue(keyPath, keyName, null);

            string? valueAsString = "";

            if (registryValue != null)
            {
                valueAsString = registryValue.ToString();
            }
            return valueAsString;
        }
    }
}
