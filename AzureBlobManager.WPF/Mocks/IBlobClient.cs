using Azure.Storage.Blobs.Models;
using System.IO;
using System.Threading.Tasks;

namespace AzureBlobManager.Mocks
{
    public interface IBlobClient
    {
        Task<Azure.Response<BlobContentInfo>> UploadAsync(FileStream fs, bool b);
        Task<Azure.Response<bool>> DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption options);
    }
}
