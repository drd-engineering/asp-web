using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoMessageSumDetail
    {
        public String TextMessage { get; set; }
        public int MessageType { get; set; } // MessageType
        public DateTime DateMessage { get; set; }
        public Boolean IsMe { get; set; }
    }
}
