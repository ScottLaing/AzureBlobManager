namespace SimpleBlobUtility
{
    public class Constants
    {
        public const string Salt = "EA5493F2-DAF0-42A5-82A8-4C5B0235CA53";
        public const string EncryptionKey = "F965F15A-DC1B-4F27-A27C-AB9C20EBC06E";
        public const int GuidLength = 36;

        public static bool EncryptNotes = true;

        public class AppGlobal
        {
            public static string ApplicationName = "Azure Blobs Accessor";
        }

        public class UIMessages
        {
            public const string GridRowEmptyError = "Could not get current grid row, is grid empty?";
            public const string GridRowObjectNotValid = "Grid row source does not appear to be a valid Cloud Blob object.";
            public const string FileItemNoContainerName = "Could not get the container name from file item.";
            public const string ContainerNotSelected = "No container item chosen or no containers available (create some in Azure Portal?).";
            public const string NotGetTempFilePathError = "Could not get temp file path.";
            public const string NoContainerSelected = "Please select a container to upload to.";
            public const string FileDeletedSuccess = "File deleted successfully.";
            public const string DeletionError = "Error occurred with deleting: {0}.";
            public const string ErrorGettingFilesList = "Error occurred with obtaining files list: {0}.";
            public const string SomeErrorOccurred = "Some error occurred.";
            public const string PleaseSelectFile = "Please select a file to upload.";
            public const string FileDoesNotExist = "File does not appear to exist. Please retry with a valid file name.";
            public const string TroubleSavingFile = "Trouble saving file to blob, {0}";
            public const string FileUploadedSuccessfully = "{0} uploaded successfully, you may now close dialog if finished.";
            public const string TextDocuments = "Text documents (*.txt)|*.txt|";
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
