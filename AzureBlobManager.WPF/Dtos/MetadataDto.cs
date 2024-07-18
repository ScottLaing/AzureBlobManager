using System.Collections.Generic;

namespace AzureBlobManager.Dtos
{
    /// <summary>
    /// Represents a metadata key-value pair.
    /// </summary>
    public class MetadataDto
    {
        /// <summary>
        /// Gets or sets the key name.
        /// </summary>
        public string KeyName { get; set; } = "";

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; } = "";

        /// <summary>
        /// Converts a dictionary of key-value pairs to a list of MetadataDto objects.
        /// </summary>
        /// <param name="keyValuePairs">The dictionary of key-value pairs.</param>
        /// <returns>A list of MetadataDto objects.</returns>
        public static List<MetadataDto> fromDictionary(Dictionary<string, string> keyValuePairs)
        {
            List<MetadataDto> metadataDtos = new List<MetadataDto>();
            foreach (var keyValuePair in keyValuePairs)
            {
                metadataDtos.Add(new MetadataDto() { KeyName = keyValuePair.Key, Value = keyValuePair.Value });
            }
            return metadataDtos;
        }

        /// <summary>
        /// Converts a list of MetadataDto objects to a dictionary of key-value pairs.
        /// </summary>
        /// <param name="metadataList">The list of MetadataDto objects.</param>
        /// <returns>A dictionary of key-value pairs.</returns>
        public static Dictionary<string, string> toDictionary(List<MetadataDto> metadataList)
        {
            var dictout = new Dictionary<string, string>();

            foreach (var item in metadataList)
            {
                dictout[item.KeyName] = item.Value;
            }
            return dictout;
        }
    }
}
