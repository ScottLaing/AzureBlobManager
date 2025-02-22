using Azure.Storage.Blobs.Models;
using System;

namespace AzureBlobManager.Mocks
{
    public interface IBlobItem
    {
        string Name { get; }
        string ContentType { get; }
        long Size { get; }
        DateTimeOffset? LastModified { get; }
        long? ContentLength { get; }
        public BlobItemProperties Properties { get; set; }
    }
}
