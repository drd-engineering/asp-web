using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoMessage
    {
        public long Id { get; set; } // Id (Primary key)
        public long FromId { get; set; } // FromId
        public int FromType { get; set; } // FromType
        public long ToId { get; set; } // ToId
        public int ToType { get; set; } // ToType
        public long? BroadcastMessageId { get; set; } // BroadcastMessageId
        public string TextMessage { get; set; } // TextMessage (length: 4000)
        public int MessageType { get; set; } // MessageType
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateOpened { get; set; } // DateOpened
        public System.DateTime? DateReplied { get; set; } // DateReplied

        public DtoBroadcastMessage BroadcastMessage { get; set; } // FK_Message_BroadcastMessage
    }
}
