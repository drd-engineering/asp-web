using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Models.Custom;

namespace DRD.Models.API
{
    public class ListWorkflowItem
    {
        public int Count { get; set; } // totalItems
        public List<WorkflowItem> Items { get; set; }
        public ListWorkflowItem()
        {
            Count = 0;
            Items = new List<WorkflowItem>();
        }
    }

}
