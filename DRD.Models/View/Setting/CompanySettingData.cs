using System.Collections.Generic;

namespace DRD.Models.View
{
    public class CompanySettingData
    {
        public ICollection<CompanyItemMember> companies;

        public CompanySettingData()
        {
            companies = new List<CompanyItemMember>();
        }

    }
    public class CompanyItemMember
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string Role { get; set; }
        public int Status { get; set; } 
        

        public CompanyItemMember()
        {
        }

    }
}
