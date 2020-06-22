namespace DRD.Models.Custom
{
    public class WorkflowNodeItem
    {
        public long Id { get; set; }    //dummy id
        public string element { get; set; }
        public string symbolCode { get; set; }
        public long? userId { get; set; }
        public string caption { get; set; }
        public string info { get; set; } // Info (length: 1000)
        public string Operator { get; set; } // Operator (length: 10)
        public string value { get; set; } // Value (length: 20)
        public string textColor { get; set; }
        public string backColor { get; set; }
        public string posLeft { get; set; } // PosLeft (length: 10)
        public string posTop { get; set; } // PosTop (length: 10)
        public string width { get; set; } // Width (length: 10)
        public string height { get; set; } // Height (length: 10)
        public WorkflowNodeUser user { get; set; }
    }
}
