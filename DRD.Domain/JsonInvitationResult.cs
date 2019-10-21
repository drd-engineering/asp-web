using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonInvitationResult
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public string SourName { get; set; }
        public string TargetName { get; set; }
    }
}
