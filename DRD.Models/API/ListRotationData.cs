using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DRD.Models.Custom;

namespace DRD.Models.API
{
    public class ListRotationData
    {
        public int Count { get; set; }
        public List<RotationData> Items { get; set; }
        public ListRotationData()
        {
            Count = 0;
            Items = new List<RotationData>();
        }
    }
}
