using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.View.Dashboard
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
            public int Rotation { get; set; }
            public int Inbox { get; set; }
            public int Altered { get; set; }
            public int Revised { get; set; }
            public int InProgress { get; set; }
            public int Pending { get; set; }
            public int Signed { get; set; }
            public int Declined { get; set; }
            public int Completed { get; set; }
            public int Contact { get; set; }
            public int Document { get; set; }
        }
    }
}


