using Azure.Storage.Blobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SimpleBlobUtility.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static SimpleBlobUtility.Constants;
using System.Windows;

namespace SimpleBlobUtility.Utils
{
    public class BlobUtility
    {
        private static string? _blobConnectionString = null;
        public static string? BlobConnectionString 
        { 
            get
            {
                if (_blobConnectionString != null)
                {
                    return _blobConnectionString;
                }   
                _blobConnectionString = Environment.GetEnvironmentVariable("AzureBlobConnectionString");
                return _blobConnectionString;
            } 
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
            string errors = "";
            try
            {
                // Replace with your connection string and container/blob names
                if (BlobConnectionString == null)
                {
                    return (false, "blob connection string is null");
                }

                string connectionString = BlobConnectionString;
                string blobName = fileName;

                BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                // This will delete the blob if it exists and include snapshots (optional)
                bool deleted = await blobClient.DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);

                if (deleted)
                {
                    return (true, "Blob deleted successfully!");
                }
                else
                {
                    return (false, "Blob not found.");
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
            string errors = "";

            var connectionString = BlobConnectionString;
            // test params for validity
            if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(downloadFilePath))
            {
                return (false, "Missing container name or file name or download file location in download blob file internal call, cannot continue.");
            }

            // Create a BlobServiceClient using the connection string
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // Get a reference to the container
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Get a reference to the blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            // Download the blob to a local file
            try
            {
                using (FileStream downloadFileStream = File.OpenWrite(downloadFilePath))
                {
                    await blobClient.DownloadToAsync(downloadFileStream);
                    res = true;
                    errors = ($"{fileName} Blob downloaded successfully!");
                }
            }
            catch (Exception ex)
            {
                res = false;
                errors = ($"Error downloading blob {fileName}: {ex.Message}");
            }
            return (res, errors);
        }

        public static async Task<List<FileListItemDto>> ListFiles(string containerName)
        {
            bool res = true;
            var result = new List<FileListItemDto>();
            string errors = string.Empty;
            try
            {
                var connectionString = BlobConnectionString;
                var serviceClient = new BlobServiceClient(connectionString);
                var containerClient = serviceClient.GetBlobContainerClient(containerName);
                var files = containerClient.GetBlobs();
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                foreach (var f in files)
                {
                    var meta = f.Metadata;
                    var props = f.Properties;
                    var len = props.ContentLength;
                    var lastMod = props.LastModified;
                    CloudBlockBlob blob = container.GetBlockBlobReference(f.Name);

                    result.Add(new FileListItemDto()
                    {
                        FileName = f.Name
                        , LastModified = lastMod
                        , FileSize = len
                        , Container = containerName
                    });
                }
            }
            catch (Exception ex)
            {
                errors = ex.Message;
                res = false;
            }
            return result;
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
