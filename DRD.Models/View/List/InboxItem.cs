using DRD.Models.API;
using System;
using System.Collections.Generic;

namespace DRD.Models.View
{
    // inbox/{id}
    public class InboxItem
    {
        public long Id { get; set; }
        public String CurrentActivity { get; set; } // rotationActivity name
        public List<RotationData> RotationLog { get; set; }
        public List<DocumentItem> Documents { get; set; }
        // public List<?> Attachments { get; set; }
    }
}