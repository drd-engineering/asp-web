using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View.Member;

using DRD.Service;
using DRD.Service.Context;

namespace DRD.Service
{
    public class MemberService
    {
        public Member getMember(long memberId)
        {
            using (var db = new ServiceContext())
            {
                return db.Members.Where(memberItem => memberItem.Id == memberId).FirstOrDefault();
            }
        }

        public Member getMember(long userId, long companyId)
        {
            using (var db = new ServiceContext())
            {
                return db.Members.Where(memberItem => memberItem.UserId == userId && memberItem.CompanyId == companyId).FirstOrDefault();
            }
        }

        public MemberList getAllAcceptedMember(long companyId)
        {
            using (var db = new ServiceContext())
            {
                CompanyService companyService = new CompanyService();
                ContactService contactService = new ContactService();

                var ownerCompanies = db.Members.Where(memberItem => memberItem.CompanyId == companyId && memberItem.isCompanyAccept == true && memberItem.isMemberAccept == true).ToList();

                var listReturn = new MemberList();
                System.Diagnostics.Debug.WriteLine("TES OWNER COMPANIES  :: " + ownerCompanies);
                foreach (Member x in ownerCompanies)
                {

                    var memberItem = new MemberItem();
                    System.Diagnostics.Debug.WriteLine("TES OWNER COMPANIES id  :: " + x.Id);
                    memberItem.Id = x.Id;
                    memberItem.CompanyId = x.CompanyId;
                    memberItem.UserId = x.UserId;
                    memberItem.Company = companyService.GetCompanyItem(memberItem.CompanyId);
                    memberItem.User = contactService.getContact(memberItem.UserId);
                    listReturn.addMember(memberItem);
                }
                //return db.Members.Where(memberItem =>  memberItem.CompanyId == companyId && memberItem.isCompanyAccept==true && memberItem.isMemberAccept==true).ToList();
                return listReturn;
            }
        }
        public MemberList getAllWaitingMember(long companyId)
        {
            using (var db = new ServiceContext())
            {
                    CompanyService companyService = new CompanyService();
                    ContactService contactService = new ContactService();

                    var ownerCompanies = db.Members.Where(memberItem => memberItem.CompanyId == companyId && memberItem.isCompanyAccept == false && memberItem.isMemberAccept == true).ToList();

                    var listReturn = new MemberList(); ;
                    System.Diagnostics.Debug.WriteLine("TES OWNER COMPANIES  :: " + ownerCompanies);
                    foreach (Member x in ownerCompanies)
                    {
                        var memberItem = new MemberItem();
                        System.Diagnostics.Debug.WriteLine("TES OWNER COMPANIES id  :: " + x.UserId);
                        memberItem.Id = x.Id;
                        memberItem.CompanyId = x.CompanyId;
                        memberItem.UserId = x.UserId;
                        memberItem.Company = companyService.GetCompanyItem(memberItem.CompanyId);
                        memberItem.User = contactService.getContact(memberItem.UserId);
                        listReturn.addMember(memberItem);
                        System.Diagnostics.Debug.WriteLine("TES OWNER COMPANIES id  :: " + memberItem.User.Name);
                    }
                    //return db.Members.Where(memberItem =>  memberItem.CompanyId == companyId && memberItem.isCompanyAccept==true && memberItem.isMemberAccept==true).ToList();
                    return listReturn;
                
                //return db.Members.Where(memberItem =>  memberItem.CompanyId == companyId).ToList();
            }
        }

        public List<Member> getAdministrators(long CompanyId)
        {
            using (var db = new ServiceContext())
            {
                return db.Members.Where(memberItem => memberItem.CompanyId == CompanyId && memberItem.IsAdministrator).ToList();
            }
        }

        public bool changeAdministratorAccess(long memberId, bool beAdmin)
        {
            using (var db = new ServiceContext())
            {
                Member member = getMember(memberId);
                member.IsAdministrator = beAdmin;

                bool result = db.Members.Add(member).IsAdministrator;
                db.SaveChanges();

                return result;
            }
        }

        public bool addMemberToCompany(long userId, long companyId)
        {
            using (var db = new ServiceContext())
            {
                Member member = getMember(userId, companyId);
                if (member != null)
                {
                    member.IsActive = true;
                }
                else
                {
                    member = new Member()
                    {
                        UserId = userId,
                        CompanyId = userId,
                        isCompanyAccept = true,
                        isMemberAccept = false,
                    };
                }
                bool result = db.Members.Add(member).IsAdministrator;
                db.SaveChanges();

                return result;
            }
        }

        public bool addCompanyToMember(long userId, long companyId)
        {
            using (var db = new ServiceContext())
            {
                Member member = getMember(userId, companyId);
                if (member != null)
                {
                    member.IsActive = true;
                }
                else
                {
                    member = new Member()
                    {
                        UserId = userId,
                        CompanyId = userId,
                        isCompanyAccept = false,
                        isMemberAccept = true,
                    };
                }
                bool result = db.Members.Add(member).IsAdministrator;
                db.SaveChanges();

                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<MemberData> GetLiteGroupAll(long userId, string topCriteria, int page, int pageSize)
        {
            return GetLiteGroupAll(userId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<MemberData> GetLiteGroupAll(long userId, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteGroupAll(userId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<MemberData> GetLiteGroupAll(long userId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "Name";

            if (!string.IsNullOrEmpty(order))
                ordering = order;

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = "";

            using (var db = new ServiceContext())
            {
                var result = new List<MemberData>();
                var contactFromPersonal = (from Contact in db.Contacts
                                           join User in db.Users on Contact.ContactItemId equals User.Id
                                           where Contact.ContactOwner.Id == userId && (topCriteria.Equals("") || tops.All(x => (User.Name + " " + User.Phone + " " + User.Email).Contains(x)))
                                           select new MemberData
                                           {
                                               Id = User.Id,
                                               Name = User.Name,
                                               Phone = User.Phone,
                                               Email = User.Email,
                                               ImageProfile = User.ImageProfile
                                           }).ToList();
                foreach(var contact in contactFromPersonal)
                {
                    result.Add(contact);
                }
                var CompanyList = (from member in db.Members
                                   join company in db.Companies on member.CompanyId equals company.Id
                                   where member.UserId == userId
                                   select new
                                   {
                                       companyId = company.Id,
                                       companyName = company.Name
                                   }).ToList();
                foreach (var hisCompany in CompanyList)
                {
                    var datafromcompanyx = (from Member in db.Members
                              join User in db.Users on Member.UserId equals User.Id
                              where Member.CompanyId == hisCompany.companyId && (topCriteria.Equals("") || tops.All(x => (User.Name + " " + User.Phone + " " + User.Email).Contains(x)))
                                            select new MemberData
                              {
                                  Id = User.Id,
                                  Name = User.Name,
                                  Phone = User.Phone,
                                  Email = User.Email,
                                  ImageProfile = User.ImageProfile,
                                  CompanyName = hisCompany.companyName
                              }).ToList();
                    foreach (var data in datafromcompanyx)
                    {
                        result.Add(data);
                    }
                }
                result = result.Skip(skip).Take(pageSize).ToList();
                return result;
            }
        }
    }
}
