using System;

namespace DRD.Models.Custom
{
    public class WorkflowNodeLinkItem
    {
        public long NodeId { get; set; }    // dummy id
        public long NodeToId { get; set; }  // dummy id
        public long firstNodeId { get; set; }
        public long endNodeId { get; set; }
        public string firstNode { get; set; }
        public string endNode { get; set; }
        public string elementFrom { get; set; }
        public string elementTo { get; set; }
        public String firstElement { get; set; }
        public String endElement { get; set; }
        public string symbolCode { get; set; }
        public string caption { get; set; }
        public string Operator { get; set; } // Operator (length: 10)
        public string value { get; set; } // Value (length: 20)
    }
}
