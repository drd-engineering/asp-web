using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DRD.Models.View.Contact;
using DRD.Service.Context;
using DRD.Models.Custom;
using DRD.Models.API.Register;
using DRD.Models;

namespace DRD.Service
{
    public class ContactService
    {
        public ContactList GetPersonalContact(UserSession user)
        {
            using (var db = new ServiceContext())
            {
                var result = (from User in db.Users
                              join Contact in db.Contacts on User.Id equals Contact.ContactItem.Id
                              where Contact.ContactOwner.Id == user.Id
                              select new ContactItem
                              {
                                  Id = User.Id,
                                  Name = User.Name,
                                  Phone = User.Phone,
                                  Email = User.Email,
                                  ImageProfile = User.ImageProfile
                              }
                              ).ToList();
                ContactList listReturned = new ContactList { Type = "Personal", Items = new List<ContactItem>() };
                int counter = 0;
                foreach (ContactItem x in result)
                {
                    listReturned.Items.Add(x);
                }
                listReturned.Count = counter;
                return listReturned;
            }
        }

        public ContactList GetContactFromCompany(UserSession user, long CompanyIdOfUser)
        {
            using (var db = new ServiceContext())
            {
                var MemberOfCompany = db.Members.Where(memberItem => memberItem.UserId == user.Id && memberItem.CompanyId == CompanyIdOfUser).ToList();
                if(MemberOfCompany.Count == 0)
                {
                    return null;
                }
                var result = (from Member in db.Members
                              join User in db.Users on Member.UserId equals User.Id
                              where Member.CompanyId == CompanyIdOfUser
                              select new ContactItem
                              {
                                  Id = User.Id,
                                  Name = User.Name,
                                  Phone = User.Phone,
                                  Email = User.Email,
                                  ImageProfile = User.ImageProfile
                              }
                              ).ToList();
                ContactList listReturned = new ContactList { Type = "Personal", Items = new List<ContactItem>() };
                int counter = 0;
                foreach (ContactItem x in result)
                {
                    listReturned.Items.Add(x);
                }
                listReturned.Count = counter;
                return listReturned;
            }
        }

        public long CountMemberOfCompany(UserSession user, long CompanyIdOfUser) {
            using (var db = new ServiceContext())
            {
                var MemberOfCompany = db.Members.Where(memberItem => memberItem.UserId == user.Id && memberItem.CompanyId == CompanyIdOfUser).ToList();

                return MemberOfCompany.Count;
            }

        }


        public CompanyList GetListOfCompany(UserSession user) {
            using (var db = new ServiceContext()) { 
                var MemberOfCompany = db.Members.Where(memberItem => memberItem.UserId == user.Id).ToList();
                long[] MemberIds = (from c in MemberOfCompany select c.UserId).ToArray();
                
                var Companies = db.Companies.Where(company => MemberIds.Contains(company.Id)).ToList();

                CompanyList companyList = new CompanyList();
                List<CompanyItem> companyItems = new List<CompanyItem>();

                foreach (Company c in Companies) {
                    CompanyItem item = new CompanyItem();

                    item.Name = c.Name;
                    item.Id = c.Id;
                    item.Code = c.Code;

                    companyItems.Add(item);
                }
                companyList.companies = companyItems;
                
                return companyList;
            }
        }



    }
}
