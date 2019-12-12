using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Models.Custom;

namespace DRD.Models.API
{
    public class ListWorkflowData
    {
        public int Count { get; set; } // totalItems
        public List<WorkflowData> Items { get; set; }
        public ListWorkflowData()
        {
            Count = 0;
            Items = new List<WorkflowData>();
        }
    }

}
