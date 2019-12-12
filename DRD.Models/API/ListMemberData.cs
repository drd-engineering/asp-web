using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Models.Custom;

namespace DRD.Models.API
{
    public class ListMemberData
    {
        public int Count { get; set; } // totalItems
        public List<MemberData> Items { get; set; }
        public ListMemberData()
        {
            Count = 0;
            Items = new List<MemberData>();
        }
    }

}
