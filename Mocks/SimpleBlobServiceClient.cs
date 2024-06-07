using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobManager.Mocks
{
    public class SimpleBlobServiceClient : IBlobServiceClient
    {
        public BlobServiceClient _blobServiceClient { get; set; }

        public SimpleBlobServiceClient(string connString)
        {
            _blobServiceClient = new BlobServiceClient(connString);
        }

        public IBlobContainerClient GetBlobContainerClient(string containerName)
        {
            BlobContainerClient concreteBlobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            return new SimpleBlobContainerClient(concreteBlobContainerClient, containerName);
        }
    }
}
