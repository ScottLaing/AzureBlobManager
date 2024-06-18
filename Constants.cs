using System;
using System.Collections.Generic;

namespace AzureBlobManager
{
    public class Constants
    {
        public const string Salt = "EA5493F2-DAF0-42A5-82A8-4C5B0235CA53";
        public const string EncryptionKey = "F965F15A-DC1B-4F27-A27C-AB9C20EBC06E";
        public const int GuidLength = 36;
        public static string RegistryCompanyName = "BrotwurstSoftware";
        public static string RegistryAppName = "AzureBlobManager62";
        public const string RegNameBlobConnectionKey = "BlobConnection";
        public const string EnvironmentVariableNameAzureBlobConnectionString = "AzureBlobConnectionString1";
        public const string RegNameEncryptionKey = "BobAndAlice";
        public const string RegSaltEncryptionKey = "SodChloride";
        public const string AppNotDefined = "App not defined, cannot continue with AttemptDownloadFileToTempFolder";

        public const string AesDefaultKey = "76AA6E93-AB72-4B33-B382-ABF77FF64C83"; // Must be at least 16 characters
        public const string AesDefaultIv = "55DCB9FA-32DB-4E"; // Must be 16 characters
        public const string KeyAndSaltMustNotBeEmpty = "Key and salt must not be empty.";
        public const string SaltMustBeAtLeast16Characters = "Salt must be at least 16 characters.";
        public const string KeyMustBe16CharactersLong = "Key must be 16 characters long.";
        public const string EncryptedTextMustNotBeBlank = "EncryptedText must not be blank.";
        public const string PlainTextMustNotBeBlank = "PlainText must not be blank.";

        public const string MissingBlobConnString = "Your Blob connection string does not appear to be set.\n\n" +
                    "The main application window will be empty of blob and container listings because of this - it will be blank. \n\n To fix this issue, " +
                    "open the Settings Window (top right icon on main window) and then enter your correct Blob connection string there.  thanks!";

        public const string MyAzureBlobManager = "My Azure Blob Manager";


        public const string FileNotUploadedYetWarning = "Quick note: any progress bar that may have just displayed is a behavior of the File Dialog chooser relating to windows file and security checks when choosing a file.  The file is Not Uploaded to Azure yet! \n\nYou may now upload the file to an Azure blob (if chosen file and path is correct) via the \"Up Arrow\" upload button near top right of window. Thanks!";

        public const string ConnectionIsNull = "connection is null";
        public const string ErrorGettingBlobMetadata = "Error getting blob metadata: {0}";

        public const string StringDidNotContainProperlyFormattedByteArray = "Stream did not contain properly formatted byte array";
        public const string DidNotReadByteArrayProperly = "Did not read byte array properly";
        public const string PlainText = "plainText";
        public const string CipherText = "cipherText";

        public const string KeyNameCannotContainWhitespace = "Key name cannot contain whitespace.";
        public const string Error = "Error";
        public const string KeyNameIsReserved = "Key name is a reserved system key name, cannot be used. Please use another keyname.";
        public const string KeyNameContainsUnallowedCharacters = "Key name contains unallowed characters.";

        public const string TroubleSavingMetadata = "Error saving metadata: {0}";
        public const string KeyNameCannotBeEmpty = "Key name cannot be empty, blob item not added.";
        public const string KeyNameAlreadyExists = "Key name already exists in metadata items. To edit an existing metadata item, select item then click edit.";
        public const string NoMetadataItemSelected = "No metadata item selected, please select a metadata item to edit.";
        public const string KeyNameCannotBeEmptyEdited = "Key name cannot be empty, blob item not edited.";
        public const string CannotDeleteSystemMetadataItems = "Cannot delete system metadata items.";

        public const string BlobItemNewKey = "NewKey";
        public const string BlobItemNewValue = "New Value";

        public const string AttemptingToUpdateMetadataButValueIsNull = "Attempting to update metadata but update value is null, cannot update.";
        public const string ErrorWithUpdatingMetadata = "Error with updating metadata: {0}";
        public const string OpeningMainWindow = "Opening Main Window.";

        // Constant string for the error message when trouble getting application reference
        public const string TroubleGettingApplicationReference = "Trouble getting application reference, cannot save to registry.";

        public const string WindowSize = "Window Size";
        public const string WindowSizeInfo = "Width: {0}, Height: {1}";

        public const string AreYouSure = "Are you sure?";
        public const string Confirmation = "Confirmation";

        public const string EncryptionKeyNotFound = "Encryption key not found in registry, created new key: {0}";
        public const string ApplicationStartup = "Application starting up.";
        public const string EncryptionKeyFound = "Encryption key found in registry {0}";
        public const string EncryptionSaltNotFound = "Encryption salt not found in registry, created new salt: {0}";
        public const string EncryptionKeySaltFound = "Encryption key salt found in registry {0}";
        public const string ApplicationCleanup = "Application cleanup going on.";
        public const string AttemptingToGetConnectionString = "Attempting to get connection string from registry: {0}.";
        public const string UsingConnectionStringFromRegistry = "Using connection string from registry: {0}.";

        public const string LogLocation = "AzureBlobManager/logs"; // The directory where log files are stored
        public const string LogFileName = "abm.log"; // The name of the log file
        public const string LogPattern = "*.log";
        public const string LogSeparator = "===============================================";
        public const string ErrorFileContentsAreLikelyInaccessible = "ERROR: [{0}] file contents are likely inaccessible (may be current or recently in-use log), please try again later.";
        public const string ErrorDetails = "ERROR DETAILS: {0}.";


        public static readonly List<string> BlobSystemKeyNames = new List<string>()
                {
                    BlobContentLength,
                    BlobContentType,
                    BlobLastModified,
                    BlobMetaDataJson
                };

        public const string BlobContentLength = "$ContentLength";
        public const string BlobContentType = "$ContentType";
        public const string BlobLastModified = "$LastModified";
        public const string BlobMetaDataJson = "$Metadata";



        public class FileDialogMsgs
        {
            public const string AllFiles = "All Files|*.*|Text Files|*.txt|JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|PNG Image|*.png";
            public const string SaveFileToLocal = "Save File to Local";
            public const string FileDialogFilterString = "{0}{1} ({2})|{2}";
            public const string CodecName = "Codec";
            public const string Files = "Files";
            public const string Sep = "|";
            public const string TextDocuments = "Text documents (*.txt)|*.txt|";
            public static readonly string SetupDialogAllFilesSettings = String.Format("{0} ({1})|{1}", "All Files", "*.*");
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
            public const string TroubleGettingContainers = "Trouble getting containers for Azure connection, possibly bad connection string or no containers created yet.";
            public const string BlobDeletedSuccessfully = "Blob deleted successfully!";
            public const string BlobNotFound = "Blob not found.";
            public const string MissingContainerName = "Missing container name in delete blob file internal call, cannot continue.";
            public const string FileNameBlobDownloadedSuccess = "{0} Blob downloaded successfully!";
            public const string ErrorDownloadingBlob = "Error downloading blob {0}: {1}";
            public const string DownloadedSuccessfully = "{0} downloaded successfully";
            public const string ErrorWithDownloading = "Error with downloading {0}: {1}";
            public const string CouldNotGetTempFilePath = "could not get temp file path";
            public const string MetadataError = "Error getting metadata for Blob [{0}], error: {1}.";
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
            // connection string now put into environment variable or will be saved into registry in encrypted format, new conn string can be now entered via
            // settings window.
            public static string LocalDBConnString = @"###############################################################";
            public static string AzureDBConnString = "################################################################";
        }
    }
}
