namespace AzureBlobManager.Mocks
{
    public interface IBlobServiceClientFactory
    {
        IBlobServiceClient CreateBlobServiceClient(string connectionString);
    }
}
