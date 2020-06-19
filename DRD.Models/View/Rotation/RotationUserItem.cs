using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DRD.Models;
using DRD.Models.Custom;

namespace DRD.Models.View
{
    public class RotationUserItem
    {
        public long Id { get; set; } // Id (Primary key)
        public long RotationId { get; set; } // RotationId
        public long WorkflowNodeId { get; set; } // WorkflowNodeId
        public long? UserId { get; set; } // MemberId
        public int FlagPermission { get; set; } // FlagPermission

        public string ActivityName { get; set; }
        public string EncryptedId { get; set; }
        public string Picture { get; set; }
        public long? Number { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public RotationUserItem()
        {
            FlagPermission = 0;
        }
    }
}
