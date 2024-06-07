using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
