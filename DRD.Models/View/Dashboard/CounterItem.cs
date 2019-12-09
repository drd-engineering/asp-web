﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.API.Dashboard
{
    public class CounterItem
    {
        public Item Old { get; set; }
        public Item New { get; set; }

        public CounterItem()
        {
            Old = new Item();
            New = new Item();
        }

        public class Item
        {
            public int InProgress { get; set; }
            public int Completed { get; set; }
            public long StorageQuota { get; set; }
            public long StorageUsage { get; set; }
        }
    }
}


