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
    public class SimpleBlobContainerClient : IBlobContainerClient
    {
        private string _containerName;

        public BlobContainerClient _blobContainerClient { get; set; }

        public SimpleBlobContainerClient(BlobContainerClient concreteBlobContainerClient, string containerName)
        {
            this._containerName = containerName;
            _blobContainerClient = concreteBlobContainerClient;
        }

        public IBlobClient GetBlobClient(string blobName)
        {
            BlobClient concreteBlobClient = _blobContainerClient.GetBlobClient(blobName);
            return new SimpleBlobClient(concreteBlobClient);

        }
    }
}
