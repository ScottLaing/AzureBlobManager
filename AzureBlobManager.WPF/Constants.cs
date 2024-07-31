using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows;

namespace AzureBlobManager
{
    // This class contains constants used in the AzureBlobManager namespace
    public class Constants
    {
        // Constants for encryption and decryption
        public const string Salt = "EA5493F2-DAF0-42A5-82A8-4C5B0235CA53";
        public const string EncryptionKey = "F965F15A-DC1B-4F27-A27C-AB9C20EBC06E";
        public const int GuidLength = 36;

        // Constants for registry settings
        public static string RegistryCompanyName = "BrotwurstSoftware";
        public static string RegistryAppName = "AzureBlobManager62";
        public const string RegNameBlobConnectionKey = "BlobConnection";
        public const string EnvironmentVariableNameAzureBlobConnectionString = "AzureBlobConnectionString1";
        
        public const string RegNameEncryptionKeyZebra = "BobAndAliceZebra";
        public const string RegSaltEncryptionKeyZebra = "SodChlorideZebra";

        public const string RegNameEncryptionKeyRoot = "BobAndAlice";
        public const string RegSaltEncryptionKeyRoot = "SodChloride";

        public const string RegNameEncryptionKey1 = "BobAndAlice1";
        public const string RegSaltEncryptionKey1 = "SodChloride1";

        public const string RegNameEncryptionKey2 = "BobAndAlice2";
        public const string RegSaltEncryptionKey2 = "SodChloride2";

        public const string RegNameEncryptionKey3 = "BobAndAlice3";
        public const string RegSaltEncryptionKey3 = "SodChloride3";

        public const string RegNameEncryptionKey4 = "BobAndAlice4";
        public const string RegSaltEncryptionKey4 = "SodChloride4";

        // Constants for AES encryption
        public const string AesDefaultKey = "76AA6E93-AB72-4B33-B382-ABF77FF64C83"; // Must be at least 16 characters
        public const string AesDefaultIv = "55DCB9FA-32DB-4E"; // Must be 16 characters

        // Constants for error messages
        public const string KeyAndSaltMustNotBeEmpty = "Key and salt must not be empty.";
        public const string SaltMustBeAtLeast16Characters = "Salt must be at least 16 characters.";
        public const string KeyMustBe16CharactersLong = "Key must be 16 characters long.";
        public const string EncryptedTextMustNotBeBlank = "EncryptedText must not be blank.";
        public const string PlainTextMustNotBeBlank = "PlainText must not be blank.";

        // Other error messages
        public const string DependencyInjectionError = "dependency injection setup error";
        public const string WindowMouseDoubleClickCall = "Window_MouseDoubleClick call";
        public const string ConnectionIsNull = "connection is null";
        public const string ErrorGettingBlobMetadata = "Error getting blob metadata: {0}";
        public const string StringDidNotContainProperlyFormattedByteArray = "Stream did not contain properly formatted byte array";
        public const string DidNotReadByteArrayProperly = "Did not read byte array properly";
        public const string PlainText = "plainText";
        public const string CipherText = "cipherText";
        public const string Error = "Error";
        public const string KeyNameIsReserved = "Key name is a reserved system key name, cannot be used. Please use another keyname.";
        public const string KeyNameContainsUnallowedCharacters = "Key name contains unallowed characters.";
        public const string BlobItemNewKey = "NewKey";
        public const string BlobItemNewValue = "New Value";
        public const string OpeningMainWindow = "Opening Main Window.";
        public const string TroubleGettingApplicationReference = "Trouble getting application reference, cannot save to registry.";
        public const string WindowSize = "Window Size";
        public const string WindowSizeInfo = "Width: {0}, Height: {1}";
        public const string EncryptionKeyNotFound = "Encryption key not found in registry, created new key: {0}";
        public const string ApplicationStartup = "Application starting up.";
        public const string EncryptionKeyFound = "Encryption key found in registry {0}";
        public const string EncryptionSaltNotFound = "Encryption salt not found in registry, created new salt: {0}";
        public const string EncryptionKeySaltFound = "Encryption key salt found in registry {0}";
        public const string ApplicationCleanup = "Application cleanup going on.";
        public const string AttemptingToGetConnectionString = "Attempting to get connection string from registry: {0}.";
        public const string UsingConnectionStringFromRegistry = "Using connection string from registry: {0}.";
        public const string LogLocation = "AzureBlobManager/logs";
        public const string LogFileName = "abm.log";
        public const string LogPattern = "*.log";
        public const string LogSeparator = "===============================================";
        public const string ErrorDetails = "ERROR DETAILS: {0}.";

        // Constants for blob system key names
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

        // Constants for file dialog messages
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

        public class SampleLargeStrings
        {
            public static readonly string[] _gettyParts = new string[] {
                "Four score and seven years ago our fathers brought forth on this continent, a new nation, conceived in Liberty, and dedicated to the proposition that",
                " all men are created equal. ",
                "Now we are engaged in a great civil war, testing whether that nation, or any nation so conceived and so dedicated, can long endure. We are met on a great battle-field of that war. We have come to dedicate a portion of that field, as a final resting place for those who here gave their lives that that nation might live. It is altogether fitting and proper that we should do this. ",
                "But, in a larger sense, we can not dedicate -- we can not consecrate -- we can not hallow -- this ground. The brave men, living and dead, who struggled here, have consecrated it, far above our poor power to add or detract. The world will little note, nor long remember what we say here, but it can never forget what they did here. It is for us the living, rather, to be dedicated here ",
                "to the unfinished work which they who fought here have thus far so nobly advanced. It is rather for us to be here dedicated to the great task remaining before us -- that from these honored dead we take increased devotion to that cause for which they gave the last full measure of devotion -- that we here highly resolve that these dead shall not have died in vain -- ",
                "that this nation, under God, shall have a new birth of freedom -- and that government of the people, by the people, for the people, shall not perish from the earth. ",
                "\n\n - Abraham Lincoln" };
            public static readonly string GettysburgAddress = string.Join("", _gettyParts);
            public static readonly string[] _churchParts = new string[]
            {
                "What General Weygand called the Battle of France is over. I expect that the Battle of Britain is about to begin. ",
                "Upon this battle depends the survival of Christian civilization. Upon it depends our own British life, and the long continuity of our ",
                "institutions and our Empire. The whole fury and might of the enemy must very soon be turned on us. Hitler knows that he will have ",
                "to break us in this Island or lose the war. If we can stand up to him, all Europe may be free and the life of the world may move forward ",
                "into broad, sunlit uplands. But if we fail, then the whole world, including the United States, including all that we have known and cared ",
                "for, will sink into the abyss of a new Dark Age made more sinister, and perhaps more protracted, by the lights of perverted science. Let us ",
                "therefore brace ourselves to our duties, and so bear ourselves that, if the British Empire and its Commonwealth last for a thousand years, ",
                "men will still say, 'This was their finest hour.' \n\n - Winston Churchill"
            };
            public static readonly string TheirFinestHour = string.Join("", _churchParts);

            private static Random rand = new Random();

            public static string SampleSpeech {
                get
                {
                    int r = rand.Next(0, 2);
                    if (r == 0)
                    {
                        return GettysburgAddress;
                    }
                    else
                    {
                        return TheirFinestHour;
                    }
                }

            }
        }

        // Constants for UI messages
        public class UIMessages
        {
            public const string TheMetadataForBlobHasBeenSaved = "The metadata for blob [{0}] has been saved.";
            public const string MyAzureBlobManager = "My Azure Blob Manager";
            public const string KeyNameCannotContainWhitespace = "Key name cannot contain whitespace.";
            public const string AreYouSure = "Are you sure?";
            public const string Confirmation = "Confirmation";
            public const string FileNotUploadedYetWarning = "Quick note: any progress bar that may have just displayed is a behavior of the File Dialog chooser relating to windows file and security checks when choosing a file.  The file is Not Uploaded to Azure yet! \n\nYou may now upload the file to an Azure blob (if chosen file and path is correct) via the \"Up Arrow\" upload button near top right of window. Thanks!";
            public const string TroubleSavingMetadata = "Error saving metadata: {0}";
            public const string KeyNameCannotBeEmpty = "Key name cannot be empty, blob item not added.";
            public const string KeyNameAlreadyExists = "Key name already exists in metadata items. To edit an existing metadata item, select item then click edit.";
            public const string ErrorWithUpdatingMetadata = "Error with updating metadata: {0}";
            public const string NoMetadataItemSelected = "No metadata item selected, please select a metadata item to edit.";
            public const string KeyNameCannotBeEmptyEdited = "Key name cannot be empty, blob item not edited.";
            public const string CannotDeleteSystemMetadataItems = "Cannot delete system metadata items.";
            public const string AttemptingToUpdateMetadataButValueIsNull = "Attempting to update metadata but update value is null, cannot update.";
            public const string MissingBlobConnString = "Your Blob connection string does not appear to be set.\n\nThe main application window will be empty of blob and container listings because of this - it will be blank. \n\nTo fix this issue, open the Settings Window (top right icon on main window) and then enter your correct Blob connection string there. Thanks!";
            public const string ErrorFileContentsAreLikelyInaccessible = "ERROR: [{0}] file contents are likely inaccessible (may be current or recently in-use log), please try again later.";
            public const string AppNotDefined = "App not defined, cannot continue with AttemptDownloadFileToTempFolder";
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
            public const string StartingUploadFileWindow = "Starting upload file window.";
            public const string UploadFileDialogFileBeingUploaded = "Upload file dialog - file being uploaded.";
            public const string UploadFileSelectingFile = "Upload file selecting file.";
            public const string StartingSettingsWindow = "Starting Settings Window.";
            public const string SavingSettings = "Saving Settings.";
            public const string SavedSettingsToRegistry = "Saved settings to registry.";
            public const string YouAreAboutToDeleteTheBlob = "You are about to delete the blob '{0}' from the container '{1}'.";
            public const string TroubleWithViewingFile = "Trouble with viewing file, Error: {0}";
            public const string NoteYouAreAboutToViewCopy = "Note: You are about to view a COPY of the latest version of the Blob [{0}]. " +
            "Changing this file you are viewing will not change the Blob stored in Azure. " +
            "To change a Blob in Azure you must reupload a modified version, " +
            "using the exact same file name, back to Azure. That would then overwrite the Azure Blob and update the Blob. To download a "
            + "Blob use the download button on the main window.";
            public const string PleaseEnterAPlainTextToEncrypt = "Please enter a plain text to encrypt.";
            public const string PleaseEnterInputText = "Please enter an input text for operation.";
            public const string PleaseEnterACypherTextToDecrypt = "Please enter a cypher text to decrypt.";

            public const string FeatureCreationInProgress = "Feature creation in progress, check back soon.";

        }
    }
}
