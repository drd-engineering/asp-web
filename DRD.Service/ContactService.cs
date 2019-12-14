
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DRD.Models.API.Contact;
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
                // Scenario:
                // login using user with id = "11111211"
                var result = (from User in db.Users
                              join Contact in db.Contacts on User.Id equals Contact.ContactItemId
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
                    counter = counter + 1;
                }

                listReturned.Count = counter;
                
                return listReturned;
            }
        }

        public long GetTotalPersonalContact(UserSession user) {
            using (var db = new ServiceContext())
            {
                // Scenario:
                // login using user with id = "11111211"
                var result = (from User in db.Users
                              join Contact in db.Contacts on User.Id equals Contact.ContactItemId
                              where Contact.ContactOwner.Id == user.Id
                              select new ContactItem
                              {
                                  Id = User.Id,
                                  Name = User.Name,
                                  Phone = User.Phone,
                                  Email = User.Email,
                                  ImageProfile = User.ImageProfile
                              }
                              ).Count();
                
                return result;
            }
        }

        public ContactList GetPersonalContact(UserSession user, string searchKey, int page, int size)
        {
            using (var db = new ServiceContext())
            {
                // Scenario:
                // login using user with id = "11111211"
                var result = (from User in db.Users
                              join Contact in db.Contacts on User.Id equals Contact.ContactItemId
                              where Contact.ContactOwner.Id == user.Id 
                                && User.Name.Contains(searchKey)
                              
                              select new ContactItem
                              {
                                  Id = User.Id,
                                  Name = User.Name,
                                  Phone = User.Phone,
                                  Email = User.Email,
                                  ImageProfile = User.ImageProfile
                              }
                              ).Skip(size * (page - 1))
                                .Take(size)
                                .ToList();
                ContactList listReturned = new ContactList { Type = "Personal", Items = new List<ContactItem>() };
                int counter = 0;
                foreach (ContactItem x in result)
                {
                    listReturned.Items.Add(x);
                    counter += 1;
                }
                listReturned.Count = counter;
                return listReturned;
            }
        }


        public ContactList GetContactFromCompany(UserSession user, long CompanyIdOfUser)
        {
            using (var db = new ServiceContext())
            {
                var hisSelfAsMember = db.Members.Where(member => member.UserId == user.Id).ToList();
                if (hisSelfAsMember.Count == 0) { return null; }
                long[] companyIds = (from m in hisSelfAsMember where CompanyIdOfUser == m.CompanyId select m.CompanyId).ToArray();
                if (companyIds.Length == 0) { return null; }

                string companyName = db.Companies.Where(c => c.Id == CompanyIdOfUser).Select(c => c.Name).FirstOrDefault();

                var result = (from Member in db.Members
                              join User in db.Users on Member.UserId equals User.Id
                              where Member.CompanyId == CompanyIdOfUser && User.Id != user.Id
                              select new ContactItem
                              {
                                  Id = User.Id,
                                  Name = User.Name,
                                  Phone = User.Phone,
                                  Email = User.Email,
                                  ImageProfile = User.ImageProfile
                              }
                              ).ToList();
                
                ContactList listReturned = new ContactList { Type = "Company", Items = new List<ContactItem>(), CompanyName = companyName};
                int counter = 0;
                foreach (ContactItem x in result)
                {
                    listReturned.Items.Add(x);
                    counter += 1;
                }
                listReturned.Count = counter;
                return listReturned;
            }
        }

        public long CountMemberOfCompany(long CompanyIdOfUser) {
            using (var db = new ServiceContext())
            {
                var MemberOfCompany = db.Members.Where(memberItem => memberItem.CompanyId == CompanyIdOfUser).ToList();

                return MemberOfCompany.Count;
            }

        }
        public ContactItem getContact(long userId)
        {
            using (var db = new ServiceContext())
            {
                var result = (from User in db.Users 
                              where User.Id == userId
                              select new ContactItem
                              {
                                  Id = User.Id,
                                  Name = User.Name,
                                  Phone = User.Phone,
                                  Email = User.Email,
                                  ImageProfile = User.ImageProfile
                              }
                             ).FirstOrDefault();
                return result;
            }
        }
        // list all company that relate to the user (a member)
        public CompanyList GetListOfCompany(UserSession user) {
            using (var db = new ServiceContext()) {
                long[] CompanyIds = db.Members.Where(member => member.UserId == user.Id).Select(c => c.CompanyId).ToArray();
                
                
                var Companies = db.Companies.Where(company => CompanyIds.Contains(company.Id)).ToList();

                CompanyList companyList = new CompanyList();
                List<CompanyItem> companyItems = new List<CompanyItem>();

                foreach (Company c in Companies) {
                    CompanyItem item = new CompanyItem();

                    item.Name = c.Name;
                    item.Id = c.Id;
                    item.Code = c.Code;

                    // get count member and exlude Current User
                    item.TotalMember = CountMemberOfCompany(c.Id) - 1;

                    companyItems.Add(item);

                    System.Diagnostics.Debug.WriteLine("LIST COMPANY: ");
                    System.Diagnostics.Debug.WriteLine(item.Name);

                }
                companyList.companies = companyItems;
                
                return companyList;
            }
        }



    }
}
