using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobManager.Mocks
{
    interface IBlobServiceClient
    {
        IBlobContainerClient GetBlobContainerClient(string containerName);
    }
}
