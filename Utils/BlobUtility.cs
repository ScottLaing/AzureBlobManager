using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SimpleBlobUtility.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using static SimpleBlobUtility.Constants;
using static SimpleBlobUtility.Constants.UIMessages;

namespace SimpleBlobUtility.Utils
{
    public class BlobUtility
    {
        /// <summary>
        /// Gets or sets the connection string for the Azure Blob Storage.
        /// </summary>
        public static string? BlobConnectionString { get; set; } = String.Empty;

        /// <summary>
        /// Initializes the BlobConnectionString property from the environment variable.
        /// </summary>
        public static void InitializeBlobConnStringFromEnvVariable()
        {
            BlobConnectionString = Environment.GetEnvironmentVariable(EnvironmentVariableNameAzureBlobConnectionString);
        }

        /// <summary>
        /// Saves a file to the specified container in Azure Blob Storage.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="filePath">The path of the file to be saved.</param>
        /// <param name="containerName">The name of the container.</param>
        /// <returns>A tuple indicating the success status and any error information.</returns>
        public static async Task<(bool, string)> SaveFileAsync(string fileName, string filePath, string containerName)
        {
            // Method comments
            // Saves a file to the specified container in Azure Blob Storage.
            //
            // Parameters:
            //   fileName:
            //     The name of the file.
            //
            //   filePath:
            //     The path of the file to be saved.
            //
            //   containerName:
            //     The name of the container.
            //
            // Returns:
            //     A tuple indicating the success status and any error information.

            bool res = true;
            string errors = string.Empty;
            try
            {
                var connectionString = BlobConnectionString;

                var serviceClient = new BlobServiceClient(connectionString);
                var containerClient = serviceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(fileName);
                using FileStream uploadFileStream = File.OpenRead(filePath);
                await blobClient.UploadAsync(uploadFileStream, true);
                uploadFileStream.Close();
            }
            catch (Exception ex)
            {
                errors = ex.Message;
                res = false;
            }
            return (res, errors);
        }

        /// <summary>
        /// Deletes a blob file from the specified container in Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <param name="fileName">The name of the file to be deleted.</param>
        /// <returns>A tuple indicating the success status and any error information.</returns>
        public static async Task<(bool success, string errorInfo)> DeleteBlobFileAsync(string containerName, string fileName)
        {
            // Method comments
            // Deletes a blob file from the specified container in Azure Blob Storage.
            //
            // Parameters:
            //   containerName:
            //     The name of the container.
            //
            //   fileName:
            //     The name of the file to be deleted.
            //
            // Returns:
            //     A tuple indicating the success status and any error information.

            bool res = true;
            string errors = string.Empty;
            try
            {
                var connectionString = BlobConnectionString;
                string blobName = fileName;

                var containerClient = new BlobContainerClient(connectionString, containerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                // This will delete the blob if it exists and include snapshots (optional)
                bool deleted = await blobClient.DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);

                if (deleted)
                {
                    return (true, BlobDeletedSuccessfully);
                }
                else
                {
                    return (false, BlobNotFound);
                }
            }
            catch (Exception ex)
            {
                errors = ex.Message;
                res = false;
            }
            return (res, errors);
        }

        /// <summary>
        /// Downloads a blob file from the specified container in Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <param name="fileName">The name of the file to be downloaded.</param>
        /// <param name="downloadFilePath">The path where the file will be downloaded.</param>
        /// <returns>A tuple indicating the success status and any error information.</returns>
        public static async Task<(bool success, string errorInfo)> DownloadBlobFileAsync(string containerName, string fileName, string downloadFilePath)
        {
            // Method comments
            // Downloads a blob file from the specified container in Azure Blob Storage.
            //
            // Parameters:
            //   containerName:
            //     The name of the container.
            //
            //   fileName:
            //     The name of the file to be downloaded.
            //
            //   downloadFilePath:
            //     The path where the file will be downloaded.
            //
            // Returns:
            //     A tuple indicating the success status and any error information.

            bool res = false;
            string errors = String.Empty;

            var connectionString = BlobConnectionString;
            // test params for validity
            if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(downloadFilePath))
            {
                return (false, MissingContainerName);
            }

            // Download the blob to a local file
            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(fileName);

                using (FileStream downloadFileStream = File.OpenWrite(downloadFilePath))
                {
                    await blobClient.DownloadToAsync(downloadFileStream);
                    res = true;
                    errors = string.Format(FileNameBlobDownloadedSuccess, fileName);
                }
            }
            catch (Exception ex)
            {
                res = false;
                errors = string.Format(ErrorDownloadingBlob, fileName, ex.Message);
            }
            return (res, errors);
        }

        /// <summary>
        /// Retrieves the metadata of a blob file from the specified container in Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <param name="blobName">The name of the blob file.</param>
        /// <returns>A tuple containing the metadata dictionary and any error information.</returns>
        public static async Task<(Dictionary<string, string> metaData, string errors)> GetBlobMetadataAsync(string containerName, string blobName)
        {
            // Method comments
            // Retrieves the metadata of a blob file from the specified container in Azure Blob Storage.
            //
            // Parameters:
            //   containerName:
            //     The name of the container.
            //
            //   blobName:
            //     The name of the blob file.
            //
            // Returns:
            //     A tuple containing the metadata dictionary and any error information.

            string connectionString = BlobConnectionString ?? throw new Exception(ConnectionIsNull);
            var result = new Dictionary<string, string>();

            try
            {
                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);
                var properties = await blobClient.GetPropertiesAsync();

                result[BlobContentLength] = properties.Value.ContentLength.ToString();
                result[BlobContentType] = properties.Value.ContentType;
                result[BlobLastModified] = properties.Value.LastModified.ToString();
                result[BlobMetaDataJson] = JsonSerializer.Serialize(properties.Value.Metadata);

                // Access user-defined metadata (if any)
                foreach (var kvp in properties.Value.Metadata)
                {
                    result[kvp.Key] = kvp.Value;
                }
            }
            catch (Exception ex)
            {
                return (new Dictionary<string, string>(), string.Format(ErrorGettingBlobMetadata, ex.Message));
            }

            return (result, "");
        }

        /// <summary>
        /// Sets the metadata of a blob file in the specified container in Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <param name="blobName">The name of the blob file.</param>
        /// <param name="newMetadata">The new metadata dictionary.</param>
        /// <returns>Any error information.</returns>
        public static async Task<string> SetBlobMetadataAsync(string containerName, string blobName, Dictionary<string, string> newMetadata)
        {
            // Method comments
            // Sets the metadata of a blob file in the specified container in Azure Blob Storage.
            //
            // Parameters:
            //   containerName:
            //     The name of the container.
            //
            //   blobName:
            //     The name of the blob file.
            //
            //   newMetadata:
            //     The new metadata dictionary.
            //
            // Returns:
            //     Any error information.

            string connectionString = BlobConnectionString ?? throw new Exception(ConnectionIsNull);

            try
            {
                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                var updateDictionary = new Dictionary<string, string>();

                foreach (var key in newMetadata.Keys)
                {
                    if (BlobSystemKeyNames.Contains(key))
                    {
                        continue;
                    }
                    updateDictionary[key] = newMetadata[key];
                }

                await blobClient.SetMetadataAsync(updateDictionary);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        /// <summary>
        /// Retrieves the list of files in the specified container in Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <returns>A tuple containing the list of file items and any error information.</returns>
        public static async Task<(List<FileListItemDto> fileItemsList, string errors)> GetContainersFileListAsync(string containerName)
        {
            // Method comments
            // Retrieves the list of files in the specified container in Azure Blob Storage.
            //
            // Parameters:
            //   containerName:
            //     The name of the container.
            //
            // Returns:
            //     A tuple containing the list of file items and any error information.

            string connectionString = BlobConnectionString ?? throw new Exception(ConnectionIsNull);
            var result = new List<FileListItemDto>();
            string errors = String.Empty;
            try
            {

                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    //var meta = blobItem.Metadata;
                    var props = blobItem.Properties;
                    var len = props.ContentLength;
                    var lastMod = props.LastModified;

                    result.Add(new FileListItemDto()
                    {
                        FileName = blobItem.Name,
                        LastModified = lastMod,
                        FileSize = len,
                        Container = containerName
                    });
                }
            }
            catch (Exception ex)
            {
                errors = ex.Message;
            }
            return (result, errors);
        }

        /// <summary>
        /// Retrieves the list of containers in Azure Blob Storage.
        /// </summary>
        /// <param name="errors">Any error information.</param>
        /// <returns>The list of container names.</returns>
        public static List<string> GetContainers(out string errors)
        {
            // Method comments
            // Retrieves the list of containers in Azure Blob Storage.
            //
            // Parameters:
            //   errors:
            //     Any error information.
            //
            // Returns:
            //     The list of container names.

            var result = new List<string>();
            errors = string.Empty;
            try
            {
                var connectionString = BlobConnectionString;
                var serviceClient = new BlobServiceClient(connectionString);
                var containers = serviceClient.GetBlobContainers();
                foreach (var cont in containers)
                {
                    result.Add(cont.Name);
                }
            }
            catch (Exception ex)
            {
                errors = ex.Message;
            }
            return result;
        }
    }
}
