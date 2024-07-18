using AzureBlobManager.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureBlobManager.Interfaces
{
    /// <summary>
    /// Interface for the BlobService class.
    /// </summary>
    public interface IBlobService
    {
        /// <summary>
        /// Gets or sets the connection string for the Azure Blob Storage.
        /// </summary>
        public string? BlobConnectionString { get; set; }

        /// <summary>
        /// Initializes the BlobConnectionString property from the environment variable.
        /// </summary>
        public void InitializeBlobConnStringFromEnvVariable();

        /// <summary>
        /// Saves a file to the specified container in Azure Blob Storage.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="filePath">The path of the file to be saved.</param>
        /// <param name="containerName">The name of the container.</param>
        /// <returns>A tuple indicating the success status and any error information.</returns>
        public Task<(bool, string)> SaveFileAsync(string fileName, string filePath, string containerName);

        /// <summary>
        /// Deletes a blob file from the specified container in Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <param name="fileName">The name of the file to be deleted.</param>
        /// <returns>A tuple indicating the success status and any error information.</returns>
        public Task<(bool success, string errorInfo)> DeleteBlobFileAsync(string containerName, string fileName);

        /// <summary>
        /// Downloads a blob file from the specified container in Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <param name="fileName">The name of the file to be downloaded.</param>
        /// <param name="downloadFilePath">The path where the file will be downloaded.</param>
        /// <returns>A tuple indicating the success status and any error information.</returns>
        public Task<(bool success, string errorInfo)> DownloadBlobFileAsync(string containerName, string fileName, string downloadFilePath);

        /// <summary>
        /// Retrieves the metadata of a blob file from the specified container in Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <param name="blobName">The name of the blob file.</param>
        /// <returns>A tuple containing the metadata dictionary and any error information.</returns>
        public Task<(Dictionary<string, string> metaData, string errors)> GetBlobMetadataAsync(string containerName, string blobName);

        /// <summary>
        /// Sets the metadata of a blob file in the specified container in Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <param name="blobName">The name of the blob file.</param>
        /// <param name="newMetadata">The new metadata dictionary.</param>
        /// <returns>Any error information.</returns>
        public Task<string> SetBlobMetadataAsync(string containerName, string blobName, Dictionary<string, string> newMetadata);

        /// <summary>
        /// Retrieves the list of files in the specified container in Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <returns>A tuple containing the list of file items and any error information.</returns>
        public Task<(List<FileListItemDto> fileItemsList, string errors)> GetContainersFileListAsync(string containerName);

        /// <summary>
        /// Retrieves the list of containers in Azure Blob Storage.
        /// </summary>
        /// <param name="errors">Any error information.</param>
        /// <returns>The list of container names.</returns>
        public List<string> GetBlobContainers(out string errors);
    }
}
