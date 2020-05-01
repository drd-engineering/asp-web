using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.View
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
            public long StorageLimit { get; set; }
            public long TotalStorage { get; set; }
        }
    }
}


