using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.View.Member
{
    public class MemberList
    {
        public ICollection<MemberItem> members { set; get; }

        public long addMember(MemberItem memberItem)
        {
            members.Add(memberItem);
            return memberItem.Id;
        }

        public MemberList()
        {
            members = new List<MemberItem>();
        }
    }
}
