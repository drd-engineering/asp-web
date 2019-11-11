using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.API
{
    public class UploadResponse
    {
        
        public string fileUrl { get; set; }
        public string fileSize { get; set; }
        public string status { get; set; }

        public UploadResponse(string fileUrl, string fileSize, string status)
        {
            
            this.fileUrl = fileUrl;
            this.fileSize = fileUrl;
            this.status = status;
        }
        public UploadResponse()
        {
            this.fileUrl = "";
            this.fileSize = "";
            this.status = "";
        }
    }
}
