using System.Collections.Generic;

namespace DRD.Models.View
{
    public class MemberList
    {
        public string status { get; set; }
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
