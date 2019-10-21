using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonCounter
    {
        public Item Old { get; set; }
        public Item New { get; set; }

        public JsonCounter()
        {
            Old = new Item();
            New = new Item();
        }

        public class Item
        {
            public int UnReadChat { get; set; }
            public int Rotation { get; set; }
            public int Inbox { get; set; }
            public int Altered { get; set; }
            public int Revised { get; set; }
            public int InProgress { get; set; }
            public int Pending { get; set; }
            public int Signed { get; set; }
            public int Declined { get; set; }
            public int Completed { get; set; }
            public decimal DrDrive { get; set; }
            public string DrDriveDesc { get; set; }
            public int Contact { get; set; }
            public int Document { get; set; }
            public decimal DepositBalance { get; set; }
        }
    }
}
