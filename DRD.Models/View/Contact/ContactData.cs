using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Models.API;

namespace DRD.Models.View
{
    // bundling all contact data for Contact page needs.
    public class ContactData
    {
        public ICollection<CompanyItem> CompanyList;
        public ContactList ContactList;
    }
}
