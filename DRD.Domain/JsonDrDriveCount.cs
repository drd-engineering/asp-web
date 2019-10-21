using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonDrDriveCount
    {
        public long PlanStorageSize { get; set; }
        public long TotalStorageUsage { get; set; }
        public List<FolderCounter> Folders { get; set; }

        public class FolderCounter
        {
            public DtoMemberFolder Folder { get; set; }
            public long TotalFile { get; set; }
            public long TotalSize { get; set; }
            public int CxDownload { get; set; }

            public FolderCounter()
            {
                Folder = new DtoMemberFolder();
            }
        }
    }
}
