using System;

namespace SimpleBlobUtility
{
    public class Constants
    {
        public const string Salt = "EA5493F2-DAF0-42A5-82A8-4C5B0235CA53";
        public const string EncryptionKey = "F965F15A-DC1B-4F27-A27C-AB9C20EBC06E";
        public const int GuidLength = 36;

        public static bool EncryptNotes = true;

        public static string CompanyName = "BrotwurstSoftware";
        public static string RegistryAppName = "AzureBlobManager62"; 


        public class AppGlobal
        {
            public static string ApplicationName = "Azure Blobs Accessor";
        }

        public class UIMessages
        {
            public const string GridRowEmptyError = "Could not get blob, valid container not selected or no blob item selected or blob items are empty.";
            public const string GridRowObjectNotValid = "Blob item does not appear to be a valid Cloud Blob object.";
            public const string FileItemNoContainerName = "Could not get the container name from blob item.";
            public const string ContainerNotSelected = "No container item chosen or no containers available (create some in Azure Portal?).";
            public const string NotGetTempFilePathError = "Could not get the windows temp file path needed for the operation.";
            public const string NoContainerSelected = "Please select a container to upload to.";
            public const string FileDeletedSuccess = "Blob deleted successfully.";
            public const string DeletionError = "Error occurred with deleting: {0}.";
            public const string ErrorGettingFilesList = "Error occurred with obtaining blobs list: {0}.";
            public const string SomeErrorOccurred = "Some error occurred.";
            public const string PleaseSelectFile = "Please select a file to upload to blob container.";
            public const string FileDoesNotExist = "File does not appear to exist. Please retry with a valid file name.";
            public const string TroubleSavingFile = "Trouble saving file to blob, {0}";
            public const string FileUploadedSuccessfully = "[{0}] uploaded successfully. You may now close dialog if finished.";
            public const string TextDocuments = "Text documents (*.txt)|*.txt|";
            public static readonly string SetupDialogAllFilesSettings = String.Format("{0} ({1})|{1}", "All Files", "*.*");
            public const string TroubleGettingContainers = "Trouble getting containers for Azure connection, possibly bad connection string or no containers created yet.";

            public const string BlobDeletedSuccessfully = "Blob deleted successfully!";
            public const string BlobNotFound = "Blob not found.";
            public const string MissingContainerName = "Missing container name in delete blob file internal call, cannot continue.";
            public const string FileNameBlobDownloadedSuccess = "{0} Blob downloaded successfully!";
            public const string ErrorDownloadingBlob = "Error downloading blob {0}: {1}";

            public const string DownloadedSuccessfully = "{0} downloaded successfully";
            public const string ErrorWithDownloading = "Error with downloading {0}: {1}";
            public const string CouldNotGetTempFilePath = "could not get temp file path";
        }

        public class TokenOptions
        {
            public const int TokenExpireHours = 24;
            public const int TokenExpireMinutes = 0;
        }

        public class DisplayStrings
        {
            public static string NotAvailable = "N/A";
        }

        public class RestStrings
        {
            public static string RestParamsMissing = "missing parameters in rest call";
        }

        public class ConnectionStrings
        {
            public static string LocalDBConnString = @"###############################################################";
            public static string AzureDBConnString = "################################################################";
        }
    }
}
