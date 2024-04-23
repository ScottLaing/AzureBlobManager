using System;

namespace SimpleBlobUtility.Dtos
{
    public class FileListItemDto
    {
        public string FileName { get; set; }
        public long ? FileSize { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public DateTime? LastModifiedFriendly
        {
            get
            {
                if (LastModified != null)
                {
                    return LastModified.Value.UtcDateTime;
                }
                return null;
            }
        }
    }
}
