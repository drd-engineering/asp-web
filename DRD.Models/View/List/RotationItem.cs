using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.View.List
{
    public class RotationItem
    {
        public enum StatusName
        {
            Open = 00,
            In_Progress = 01,
            Pending= 02,
            Signed = 03,
            Revision = 05,
            Altered = 06,
            Completed = 90,
            Declined = 98,
            Canceled = 99,
            Waiting_For_Response = 10,
            Accepted = 11,
            Expired = 97
        }

        public string Key { get; set; }
        public long Id { get; set; } // Id (Primary key)
        public string Subject { get; set; } // Subject (length: 2)
        public long WorkflowId { get; set; } // WorkflowId
        public int Status { get; set; } // Status (length: 2)
        public long UserId { get; set; } // MemberId
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated
        public System.DateTime? DateStatus { get; set; } // DateUpdated
        public System.DateTime? DateStarted { get; set; } // DateStarted

        public long RotationNodeId { get; set; }
        public string ActivityName { get; set; }
        public string WorkflowName { get; set; }
        public string StatusDescription { get; set; }

        public virtual System.Collections.Generic.ICollection<RotationNode> RotationNodes { get; set; } // RotationNode.FK_RotationNode_Rotation

        public RotationItem()
        {
            RotationNodes = new System.Collections.Generic.List<RotationNode>();
            
        }
        public static string getStatusName(int statusCode)
        {
            return Enum.GetName(typeof(StatusName), statusCode).Replace("_"," ");
        }
    }
}
