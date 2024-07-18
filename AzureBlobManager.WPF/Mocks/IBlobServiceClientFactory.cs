using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobManager.Mocks
{
    public interface IBlobServiceClientFactory
    {
        IBlobServiceClient CreateBlobServiceClient(string connectionString);
    }
}
