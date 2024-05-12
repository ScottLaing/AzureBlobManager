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
        public static string? BlobConnectionString { get; set; } = String.Empty;

        public static void InitializeBlobConnStringFromEnvVariable()
        {
            BlobConnectionString = Environment.GetEnvironmentVariable(EnvironmentVariableNameAzureBlobConnectionString);
        }

        public static async Task<(bool, string)> SaveFile(string fileName, string filePath, string containerName)
        {
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

        public static async Task<(bool success, string errorInfo)> DeleteBlobFile(string containerName, string fileName)
        {
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

        public static async Task<(bool success, string errorInfo)> DownloadBlobFile(string containerName, string fileName, string downloadFilePath)
        {
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

        // data prefers the term artificial person
        public static async Task<(Dictionary<string,string> metaData, string errors)> GetBlobMetadata(string containerName, string blobName)
        {
            string connectionString = BlobConnectionString ?? throw new Exception("connection is null");
            var result = new Dictionary<string, string>();

            try
            {
                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);
                var properties = await blobClient.GetPropertiesAsync();

                result["$contentLength"] = properties.Value.ContentLength.ToString();
                result["$contentType"] = properties.Value.ContentType;
                result["$lastModified"] = properties.Value.LastModified.ToString();
                result["$metadata"] = JsonSerializer.Serialize(properties.Value.Metadata);

                // Access user-defined metadata (if any)
                foreach (var kvp in properties.Value.Metadata)
                {
                    result[kvp.Key] = kvp.Value;
                }
            }
            catch (Exception ex)
            {
                return (new Dictionary<string, string>(), $"Error getting blob metadata: {ex.Message}");
            }

            return (result, "");
        }

        public static async Task<(List<FileListItemDto> fileItemsList, string errors)> GetContainersFileList(string containerName)
        {
            string connectionString = BlobConnectionString ?? throw new Exception("connection is null");
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

        public static List<string> GetContainers(out string errors)
        {
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
