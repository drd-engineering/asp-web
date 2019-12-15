using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.View.Workflow
{
    public class WorkflowNodeLinkData
    {
        public long NodeId { get; set; }    // dummy id
        public long NodeToId { get; set; }  // dummy id
        public string elementFrom { get; set; }
        public string elementTo { get; set; }
        public string symbolCode { get; set; }
        public string caption { get; set; }
        public string Operator { get; set; } // Operator (length: 10)
        public string value { get; set; } // Value (length: 20)} 
    }
}
