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
    public class SimpleBlobClient : IBlobClient
    {
        public BlobClient _blobClient { get; set; } = null!;

        public SimpleBlobClient(BlobClient concreteBlobClient)
        {
            _blobClient = concreteBlobClient;
        }

        public Task<Azure.Response<bool>> DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption options)
        {
            return _blobClient.DeleteIfExistsAsync(options);
        }

        public Task<Azure.Response<BlobContentInfo>> UploadAsync(FileStream fs, bool b)
        {
            return _blobClient.UploadAsync(fs, b);
        }
    }
}
