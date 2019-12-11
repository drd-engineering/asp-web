using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public bool checkIsAdmin(long userId, long companyId)
        {
            using (var db = new ServiceContext())
            {
               var admin = db.Members.Where(memberItem => memberItem.CompanyId == companyId && memberItem.IsAdministrator && memberItem.UserId == userId).FirstOrDefault();
                return admin == null ? false : true;
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
        public ListMemberData FindMembers(long userId, string topCriteria, int page, int pageSize)
        {
            Expression<Func<MemberData, bool>> criteriaUsed = WorkflowData => true; 
            return FindMembers(userId, topCriteria, page, pageSize, null, criteriaUsed);
        }
        public ListMemberData FindMembers(long userId, string topCriteria, int page, int pageSize, Expression<Func<MemberData, string>> order)
        {
            Expression<Func<MemberData, bool>> criteriaUsed = WorkflowData => true;
            return FindMembers(userId, topCriteria, page, pageSize, order, criteriaUsed);
        }
        public ListMemberData FindMembers(long userId, string topCriteria, int page, int pageSize, Expression<Func<MemberData, string>> order, Expression<Func<MemberData, bool>> criteria)
        {
            int skip = pageSize * (page - 1);
            Expression<Func<MemberData, string>> ordering = MemberData => "Name";

            if (order != null)
                ordering = order;

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = "";

            using (var db = new ServiceContext())
            {
                var result = new ListMemberData();
                var contactListAllMatch = (from Contact in db.Contacts
                                           join User in db.Users on Contact.ContactItemId equals User.Id
                                           where Contact.ContactOwner.Id == userId && (topCriteria.Equals("") || tops.All(x => (User.Name + " " + User.Phone + " " + User.Email).Contains(x)))
                                           select new MemberData
                                           {
                                               Id = User.Id,
                                               Name = User.Name,
                                               Phone = User.Phone,
                                               Email = User.Email,
                                               ImageProfile = User.ImageProfile
                                           }).Union(from member1 in db.Members
                                                    join company in db.Companies on member1.CompanyId equals company.Id
                                                    join member2 in db.Members on company.Id equals member2.CompanyId
                                                    join user in db.Users on member2.UserId equals user.Id
                                                    where member1.UserId == userId
                                                    && member1.IsActive && member1.isCompanyAccept && member1.isMemberAccept
                                                    && member2.IsActive && member2.isCompanyAccept && member2.isMemberAccept
                                                    && (topCriteria.Equals("") || tops.All(x => (user.Name + " " + user.Phone + " " + user.Email).Contains(x)))
                                                    select new MemberData
                                                    {
                                                        Id = user.Id,
                                                        Name = user.Name,
                                                        Phone = user.Phone,
                                                        Email = user.Email,
                                                        ImageProfile = user.ImageProfile
                                                    }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();
                ListMemberData returnValue = new ListMemberData();
                if (contactListAllMatch != null)
                {
                    int counterMember = 0;
                    foreach(MemberData item in contactListAllMatch)
                    {
                        counterMember += 1;
                        returnValue.Items.Add(item);
                    }
                    returnValue.Count = counterMember;
                }
                return result;
            }
        }
    }
}
