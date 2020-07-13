
using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DRD.Service
{
    public class ContactService
    {
        /// <summary>
        /// This function used to get all personal contact that related to user, and use search key and pagination if needed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// 
        public ContactList GetPersonalContact(UserSession user, string topCriteria, int page, int pageSize)
        {
            Expression<Func<ContactItem, bool>> criteriaUsed = ContactItem => true;
            return GetPersonalContact(user, topCriteria, page, pageSize, null, criteriaUsed);
        }
        public ContactList GetPersonalContact(UserSession user, string topCriteria, int page, int pageSize, Expression<Func<ContactItem, string>> order, Expression<Func<ContactItem, bool>> criteria)
        {
            int skip = pageSize * (page - 1);
            Expression<Func<ContactItem, string>> ordering = ContactItem => "Name";

            if (order != null)
                ordering = order;

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = "";

            using (var db = new Connection())
            {
                // Scenario:
                // login using user with id = "11111211"
                var result = (from User in db.Users
                              join Contact in db.Contacts on User.Id equals Contact.ContactItemId
                              where Contact.ContactOwner.Id == user.Id
                              && (topCriteria.Equals("") || tops.All(x => (User.Name + " " + User.Email).Contains(x)))
                              select new ContactItem
                              {
                                  Id = User.Id,
                                  Name = User.Name,
                                  Phone = User.Phone,
                                  Email = User.Email,
                                  ImageProfile = User.ProfileImageFileName
                              }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();
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

        public long GetTotalPersonalContact(UserSession user)
        {
            using (var db = new Connection())
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
                                  ImageProfile = User.ProfileImageFileName
                              }
                              ).Count();

                return result;
            }
        }

        public ContactList GetContactFromCompany(UserSession user, long CompanyIdOfUser, string topCriteria, int page, int pageSize)
        {
            Expression<Func<ContactItem, bool>> criteriaUsed = ContactItem => true;
            return GetContactFromCompany(user, CompanyIdOfUser, topCriteria, page, pageSize, null, criteriaUsed);
        }
        public ContactList GetContactFromCompany(UserSession user, long CompanyIdOfUser, string topCriteria, int page, int pageSize, Expression<Func<ContactItem, string>> order, Expression<Func<ContactItem, bool>> criteria)
        {
            int skip = pageSize * (page - 1);
            Expression<Func<ContactItem, string>> ordering = ContactItem => "Name";

            if (order != null)
                ordering = order;

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = "";

            using (var db = new Connection())
            {
                var hisSelfAsMember = db.Members.Where(member => member.UserId == user.Id).ToList();
                if (hisSelfAsMember.Count == 0) { return null; }
                long[] companyIds = (from m in hisSelfAsMember where CompanyIdOfUser == m.CompanyId select m.CompanyId).ToArray();
                if (companyIds.Length == 0) { return null; }

                string companyName = db.Companies.Where(c => c.Id == CompanyIdOfUser).Select(c => c.Name).FirstOrDefault();

                var result = (from Member in db.Members
                              join User in db.Users on Member.UserId equals User.Id
                              where Member.CompanyId == CompanyIdOfUser
                              && User.Id != user.Id
                              && (topCriteria.Equals("") || tops.All(x => (User.Name + " " + User.Email).Contains(x)))
                              select new ContactItem
                              {
                                  Id = User.Id,
                                  Name = User.Name,
                                  Phone = User.Phone,
                                  Email = User.Email,
                                  ImageProfile = User.ProfileImageFileName
                              }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                ContactList listReturned = new ContactList { Type = "Company", Items = new List<ContactItem>(), CompanyName = companyName };
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

        public long CountMemberOfCompany(long CompanyIdOfUser)
        {
            using (var db = new Connection())
            {
                var MemberOfCompany = db.Members.Where(memberItem => memberItem.CompanyId == CompanyIdOfUser).ToList();

                return MemberOfCompany.Count;
            }

        }
        public ContactItem getContact(long userId)
        {
            using (var db = new Connection())
            {
                var result = (from User in db.Users
                              where User.Id == userId
                              select new ContactItem
                              {
                                  Id = User.Id,
                                  Name = User.Name,
                                  Phone = User.Phone,
                                  Email = User.Email,
                                  ImageProfile = User.ProfileImageFileName
                              }
                             ).FirstOrDefault();
                return result;
            }
        }
        // list all company that relate to the user (a member)
        public ICollection<CompanyItem> GetListOfCompany(UserSession user)
        {
            using (var db = new Connection())
            {
                long[] CompanyIds = db.Members.Where(member => member.UserId == user.Id).Select(c => c.CompanyId).ToArray();

                var Companies = db.Companies.Where(company => CompanyIds.Contains(company.Id)).ToList();

                List<CompanyItem> companyItems = new List<CompanyItem>();

                foreach (Company c in Companies)
                {
                    CompanyItem item = new CompanyItem();

                    item.Name = c.Name;
                    item.Id = c.Id;
                    item.Code = c.Code;

                    // get count member and exlude Current User
                    item.TotalMember = CountMemberOfCompany(c.Id) - 1;

                    companyItems.Add(item);

                }
                return companyItems;
            }
        }



    }
}
