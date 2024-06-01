using System;

namespace AzureBlobManager.Dtos
{
    public class FileListItemDto
    {
        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; } = "";

        /// <summary>
        /// Gets or sets the file size in bytes.
        /// </summary>
        public long? FileSize { get; set; }

        /// <summary>
        /// Gets or sets the last modified date and time in UTC.
        /// </summary>
        public DateTimeOffset? LastModified { get; set; }

        /// <summary>
        /// Gets the last modified date and time in local time zone.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the container name.
        /// </summary>
        public string Container { get; set; } = "";

        /// <summary>
        /// Performs a foreach loop over the file list items.
        /// </summary>
        /// <param name="action">The action to be performed on each file list item.</param>
        public void ForEach(Action<FileListItemDto> action)
        {
            // TODO: Implement the foreach loop logic here
        }
    }
}
