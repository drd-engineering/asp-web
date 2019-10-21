using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonActivityResult
    {
        public int ExitCode { get; set; }
        public long MemberId { get; set; }
        public string MemberName { get; set; }
        public string Email { get; set; }
    }
}
