using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SimpleBlobUtility.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;


namespace SimpleBlobUtility.Utils
{
    public class BlobUtility
    {
        private static string? _blobConnectionString = null;
        public static string BlobConnectionString 
        { 
            get
            {
                if (_blobConnectionString != null)
                {
                    return _blobConnectionString;
                }   
                _blobConnectionString = Environment.GetEnvironmentVariable("AzureBlobConnectionString");
                if (string.IsNullOrEmpty(_blobConnectionString))
                {
                    throw new InvalidOperationException("Azure ConnectionString is null or empty, cannot complete blob operation.");
                }
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
            string errors = string.Empty;
            try
            {
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

            // Download the blob to a local file
            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

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

        public static async Task<(List<FileListItemDto> fileItemsList, string errors)> ListFiles(string containerName)
        {
            string connectionString = BlobConnectionString;
            var result = new List<FileListItemDto>();
            string errors = "";
            try
            {

                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

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
