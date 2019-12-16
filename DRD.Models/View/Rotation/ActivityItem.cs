﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.View
{
    public class ActivityItem
    {
        public int ExitCode { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string RotationName { get; set; }
        public long RotationNodeId { get; set; }
        public ActivityItem() { }
    }
}