using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models
{
    public class DocumentUser
    {
        public long Id { get; set; } // Id (Primary key)
        public long DocumentId { get; set; } // DocumentId
        public long UserId { get; set; } // MemberId
        public int FlagPermission { get; set; } // FlagPermission
        public int FlagAction { get; set; } // FlagAction
        public string UserName { get; set; }

        public DocumentUser()
        {
            FlagPermission = 0;
        }
    }
}
