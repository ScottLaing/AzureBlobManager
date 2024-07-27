using AzureBlobManager.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using static AzureBlobManager.Constants;

namespace AzureBlobManager.Services
{
    /// <summary>
    /// Provides methods for saving and retrieving values from the Windows Registry.
    /// </summary>
    public class RegService : IRegService
    {
        /// <summary>
        /// Property to get the registry subkey string.
        /// </summary>
        public string RegSubKey => $"Software\\{Constants.RegistryCompanyName}\\{Constants.RegistryAppName}";

        /// <summary>
        /// Saves a value to the Windows Registry under the specified key name.
        /// </summary>
        /// <param name="keyName">The name of the registry key.</param>
        /// <param name="keyValue">The value to be saved.</param>
        public void SaveValueToRegistry(string keyName, string keyValue)
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
        public string? GetValueFromRegistry(string keyName)
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

        /// <summary>
        /// Creates the initial encryption keys if they do not exist.
        /// </summary>
        public void CreateInitialEncryptionKeys()
        {
            string newKey;
            string newSalt;

            // keys 2-4 are secondary encryption keys to allow some choice in encryption.  values will be stored in the registry.
            // New options on the encryption window will allow user to choose which encryption key they want to use.
            for (int i = 1; i < 5; i++)
            {
                string keyName = $"{RegNameEncryptionKeyRoot}{i}";
                string saltName = $"{RegSaltEncryptionKeyRoot}{i}";
                if (string.IsNullOrWhiteSpace(GetValueFromRegistry(keyName)) ||
                    string.IsNullOrWhiteSpace(GetValueFromRegistry(saltName)))
                {
                    newKey = Guid.NewGuid().ToString();
                    newSalt = Guid.NewGuid().ToString();

                    SaveValueToRegistry(keyName, newKey);
                    SaveValueToRegistry(saltName, newSalt);
                }
            }
        }

        /// <summary>
        /// Retrieves the encryption keys from the registry or generates new ones if they are not found.
        /// </summary>
        /// <param name="salts">Matching salts list for returned encryption keys list.</param>
        /// <returns></returns>
        public List<string> GetEncryptionKeys(out List<string> salts)
        {
            var keys = new List<string>();
            salts = new List<string>();

            string newKey;
            string newSalt;

            for (int i = 1; i < 5; i++)
            {
                string keyName = $"{RegNameEncryptionKeyRoot}{i}";
                string saltName = $"{RegSaltEncryptionKeyRoot}{i}";
                if (string.IsNullOrWhiteSpace(GetValueFromRegistry(keyName)) ||
                    string.IsNullOrWhiteSpace(GetValueFromRegistry(saltName)))
                {
                    newKey = Guid.NewGuid().ToString();
                    newSalt = Guid.NewGuid().ToString();

                    SaveValueToRegistry(keyName, newKey);
                    SaveValueToRegistry(saltName, newSalt);

                    keys.Add(newKey);
                    salts.Add(newSalt);
                }
                else
                {
                    keys.Add(GetValueFromRegistry(keyName) ?? "");
                    salts.Add(GetValueFromRegistry(saltName) ?? "");
                }
            }
            return keys;
        }

    }
}
