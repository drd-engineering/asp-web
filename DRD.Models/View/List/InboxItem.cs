﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.API.List
{
    // inbox/{id}
    public class InboxItem
    {
        public long Id { get; set; }
        public String CurrentActivity { get; set; } // rotationActivity name
        public List<RotationItem> RotationLog { get; set; }
        public List<DocumentItem> Documents { get; set; }
        // public List<?> Attachments { get; set; }
    }
}