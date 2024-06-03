namespace AzureBlobManager.Interfaces
{
    public interface IRegService
    {
        public string RegSubKey { get; }

        /// <summary>
        /// Saves a value to the Windows Registry under the specified key name.
        /// </summary>
        /// <param name="keyName">The name of the registry key.</param>
        /// <param name="keyValue">The value to be saved.</param>
        public void SaveValueToRegistry(string keyName, string keyValue);


        /// <summary>
        /// Retrieves the value from the Windows Registry under the specified key name.
        /// </summary>
        /// <param name="keyName">The name of the registry key.</param>
        /// <returns>The value retrieved from the registry, or an empty string if the key does not exist.</returns>
        public string? GetValueFromRegistry(string keyName);
      
    }
}
