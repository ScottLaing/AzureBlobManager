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
