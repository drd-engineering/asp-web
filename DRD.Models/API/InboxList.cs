﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.API
{
    // inbox/inboxLisst
    public class InboxList
    {
        public long Id { get; set; }
        public bool IsUnread { get; set; }
        public String RotationName { get; set; } // access rotation activity first then the rotation to get the name
        public String CurrentActivity { get; set; } // rotationActivity name
        public String WorkflowName { get; set; }
        public DateTime CreatedAt { get; set; }
        
    }
}