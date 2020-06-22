using System.Collections.Generic;

namespace DRD.Models.View
{
    public class ContactList
    {
        public string Type { get; set; }
        public string CompanyName { get; set; }
        public int Count { get; set; }
        public List<ContactItem> Items { get; set; }
    }
}
