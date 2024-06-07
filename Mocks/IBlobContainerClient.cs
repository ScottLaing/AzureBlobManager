using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobManager.Mocks
{
    public interface IBlobContainerClient
    {
        IBlobClient GetBlobClient(string blobName);

        AsyncPageable<IBlobItem> GetBlobsAsync();

    }
}
