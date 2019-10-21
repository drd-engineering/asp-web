using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonPermission
    {
        public long Id { get; set; }
        public long MemberId { get; set; }
        public bool IsAll { get; set; }
        public string UserId { get; set; }

        public DtoMemberLite Member { get; set; }

        public virtual System.Collections.Generic.ICollection<JsonPermissionCompany> Companies { get; set; }

        public JsonPermission()
        {
            Member = new DtoMemberLite();
        }
    }
}
