using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonWorkflowNode
    {
        //{ element: '', memberId: 0, symbolCode: '', caption: '', info: '', operator: '', value: 0, textColor: '', backColor: '', posLeft: 0, posTop: 0, width: 0, height: 0, member: { number: '', name: '', email: '', imageProfile: '' } };
        public long Id { get; set; }    //dummy id
        public string element { get; set; }
        public string symbolCode { get; set; }
        public long? memberId { get; set; } 
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
        public JsonWorkflowNodeMember member { get; set; }
    }
}
