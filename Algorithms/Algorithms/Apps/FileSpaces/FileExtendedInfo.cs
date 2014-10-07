using System.Collections.Generic;

namespace Algorithms.Apps.FileSpaces
{
    public class FileExtendedInfo
    {
        public string FileName { get; set; }
        public string DirectoryName { get; set; }

        public bool IsDirectory { get; set; }

        public long FileSize { get; set; }
        public long DirectorySize { get; set; }

        public FileExtendedInfo Parent { get; set; }
        public List<FileExtendedInfo> Children { get; set; }
    }
}