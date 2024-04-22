using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
