using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models
{
    public class Symbol
    {
        public int Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 50)
        public string Name { get; set; } // Name (length: 100)
        public string Descr { get; set; } // Descr
        public string Image { get; set; } // Image (length: 100)
        public int SymbolType { get; set; } // SymbolType
        public string UserId { get; set; } // UserId (length: 50)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        //// Reverse navigation
        //public virtual System.Collections.Generic.ICollection<WorkflowNode> WorkflowNodes { get; set; } // WorkflowNode.FK_WorkflowNode_Symbol
        //public virtual System.Collections.Generic.ICollection<WorkflowNodeLink> WorkflowNodeLinks { get; set; } // WorkflowNodeLink.FK_WorkflowNodeLink_Symbol

        public Symbol()
        {
            SymbolType = 0;
            //WorkflowNodes = new System.Collections.Generic.List<WorkflowNode>();
            //WorkflowNodeLinks = new System.Collections.Generic.List<WorkflowNodeLink>();
        }
    }
}
