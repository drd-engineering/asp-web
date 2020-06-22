using DRD.Models.API;
using System.Collections.Generic;

namespace DRD.Models.View
{
    // bundling all contact data for Contact page needs.
    public class ContactData
    {
        public ICollection<CompanyItem> CompanyList;
        public ContactList ContactList;
    }
}
