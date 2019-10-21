using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoMemberMaster
    {
        public List<JsonUserGroup> UserGroups { get; set; }
        public List<DtoMemberTitle> MemberTitles { get; set; }
    }
}
