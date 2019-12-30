using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.View
{
    public class UploadResponse
    {
        public long Idx; // ?? -> type file: KTP, ImageProfile, ImageSignature. Any better solution?
        public string Filename;
        public string Fileext; // ??
    }
}
