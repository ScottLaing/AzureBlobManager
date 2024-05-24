using System.Collections.Generic;

namespace SimpleBlobUtility.Dtos
{
    public class MetadataDto
    {
        public string KeyName { get; set; } = "";
        public string Value { get; set; } = "";

        public static List<MetadataDto> fromDictionary(Dictionary<string, string> keyValuePairs)
        {
            List<MetadataDto> metadataDtos = new List<MetadataDto>();
            foreach (var keyValuePair in keyValuePairs)
            {
                metadataDtos.Add(new MetadataDto() { KeyName = keyValuePair.Key, Value = keyValuePair.Value });
            }
            return metadataDtos;
        }

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
