using Azure;

namespace AzureBlobManager.Mocks
{
    public interface IBlobContainerClient
    {
        IBlobClient GetBlobClient(string blobName);

        AsyncPageable<IBlobItem> GetBlobsAsync();

    }
}
