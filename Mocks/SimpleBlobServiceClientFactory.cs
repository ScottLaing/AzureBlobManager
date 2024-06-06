using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobManager.Mocks
{
    class SimpleBlobServiceClientFactory: IBlobServiceClientFactory
    {
        public IBlobServiceClient CreateBlobServiceClient(string connectionString)
        {
            return new SimpleBlobServiceClient(connectionString);
        }
    }
}
