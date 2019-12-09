using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.API.Contact
{
    public class ContactList
    {
        public string Type { get; set; }
        public string CompanyName { get; set; }
        public int Count { get; set; }
        public List<ContactItem> Items { get; set; }
    }
}
