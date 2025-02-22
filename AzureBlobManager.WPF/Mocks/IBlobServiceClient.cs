namespace AzureBlobManager.Mocks
{
    public interface IBlobServiceClient
    {
        IBlobContainerClient GetBlobContainerClient(string containerName);
    }
}
