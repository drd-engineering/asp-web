using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoMessageSum
    {
        public long Id { get; set; }
        public string SenderFoto { get; set; }
        public long SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderProfession { get; set; }
        public int SenderType { get; set; }

        public long ReceiverId { get; set; }
        public string ReceiverName { get; set; }

        public int Unread { get; set; }
        public DateTime TheDate { get; set; }
    }
}
