﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;

using DRD.Service;
using DRD.Service.Context;

namespace DRD.Service
{
    public class MemberService
    {
        private CompanyService companyService;
        private ContactService contactService;

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
        public void mappingMember(List<Member> membersFromDb, MemberList listReturn)
        {
            companyService = new CompanyService();
            contactService = new ContactService();

            foreach (Member x in membersFromDb)
            {

                var memberItem = new MemberItem();
                memberItem.Id = x.Id;
                memberItem.CompanyId = x.CompanyId;
                memberItem.UserId = x.UserId;
                memberItem.Company = companyService.GetCompanyItem(memberItem.CompanyId);
                memberItem.User = contactService.getContact(memberItem.UserId);
                System.Diagnostics.Debug.WriteLine("USER MEMBER :: " + memberItem.User.Name);

                listReturn.addMember(memberItem);
            }
        }
        public MemberList getAcceptedMember(long companyId)
        {
            using (var db = new ServiceContext())
            {
                var listReturn = new MemberList();
                var membersFromDb = db.Members.Where(memberItem => memberItem.CompanyId == companyId && memberItem.isCompanyAccept  && memberItem.isMemberAccept && memberItem.IsActive).ToList();

                mappingMember(membersFromDb, listReturn);
                return listReturn;
            }
        }
        public MemberList getWaitingMember(long companyId)
        {
            using (var db = new ServiceContext())
            {
                var listReturn = new MemberList(); ;
                var membersFromDb = db.Members.Where(memberItem => memberItem.CompanyId == companyId && !memberItem.isCompanyAccept && memberItem.isMemberAccept && memberItem.IsActive).ToList();

                mappingMember(membersFromDb, listReturn);
                return listReturn;   
            }
        }

        public MemberList getAcceptedMember(long companyId, bool isAdmin)
        {
            using (var db = new ServiceContext())
            {
                var listReturn = new MemberList(); ;
                var membersFromDb = db.Members.Where(memberItem => memberItem.CompanyId == companyId && memberItem.isCompanyAccept && memberItem.isMemberAccept && memberItem.IsActive && memberItem.IsAdministrator == isAdmin).ToList();

                mappingMember(membersFromDb, listReturn);
                return listReturn;
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

        public List<MemberItem> getAdministrators(long CompanyId)
        {
            using (var db = new ServiceContext())
            {
                var admins = (from Member in db.Members
                              join User in db.Users on Member.UserId equals User.Id
                              where Member.CompanyId == CompanyId && Member.isCompanyAccept && Member.isMemberAccept && Member.IsActive && Member.IsAdministrator
                              select new MemberItem
                              {
                                  Id = Member.Id,
                                  User = new ContactItem
                                  {
                                      Id = User.Id,
                                      Name = User.Name
                                  }
                              }).ToList(); ;
                return admins;
                //return db.Members.Where(memberItem => memberItem.CompanyId == CompanyId && memberItem.IsAdministrator).ToList();
            }
        }

        public List<Member> getMemberByCompanyAdmin(long adminId)
        {
            using (var db = new ServiceContext())
            {

                return db.Members.Where(memberItem => memberItem.UserId == adminId && memberItem.IsActive & memberItem.IsAdministrator).ToList();
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

        public long Delete(long memberId)
        {
            using (var db = new ServiceContext())
            {
                Member member = db.Members.Where(memberItem => memberItem.Id == memberId).FirstOrDefault();
                member.IsActive = false;
                db.SaveChanges();

                return member.UserId;
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

        public ListMemberData FindMembers(long userId, string topCriteria, int page, int pageSize){
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
                                           orderby User.Name
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
                                                    orderby user.Name
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
                return returnValue;
            }
        }
        public MemberList BecomeAdmin(long companyId, ICollection<MemberItem> adminCandidate)
        {
            MemberList data = new MemberList();
            using (var db = new ServiceContext())
            {
                foreach (MemberItem x in adminCandidate)
                {
                    Member candidate = db.Members.FirstOrDefault(item => item.Id == x.Id && !item.IsAdministrator && item.IsActive && x.CompanyId == companyId);
                    if (candidate != null)
                    {
                        candidate.IsAdministrator = true;
                        db.SaveChanges();
                        var memberItem = new MemberItem();
                        memberItem.Id = candidate.Id;
                        memberItem.CompanyId = candidate.CompanyId;
                        memberItem.UserId = candidate.UserId;
                        memberItem.IsActive = candidate.IsActive;
                        memberItem.IsAdministrator = candidate.IsAdministrator;
                        memberItem.isCompanyAccept = candidate.isCompanyAccept;
                        memberItem.isMemberAccept = candidate.isMemberAccept;
                        memberItem.JoinedAt = candidate.JoinedAt;
                        memberItem.User = x.User;
                        memberItem.Company = x.Company;
                        data.addMember(memberItem);
                    }
                    else
                    {
                        data.addMember(x);
                    }
                }
            }
            return data;
        }

        public MemberList BecomeMember(long companyId, ICollection<MemberItem> memberCandidate)
        {
            MemberList data = new MemberList();
            using (var db = new ServiceContext())
            {
                foreach (MemberItem x in memberCandidate)
                {
                    Member candidate = db.Members.FirstOrDefault(item => item.Id == x.Id && item.IsAdministrator && item.IsActive && x.CompanyId == companyId);
                    if (candidate != null)
                    {
                        candidate.IsAdministrator = false;
                        db.SaveChanges();
                        var memberItem = new MemberItem();
                        memberItem.Id = candidate.Id;
                        memberItem.CompanyId = candidate.CompanyId;
                        memberItem.UserId = candidate.UserId;
                        memberItem.IsActive = candidate.IsActive;
                        memberItem.IsAdministrator = candidate.IsAdministrator;
                        memberItem.isCompanyAccept = candidate.isCompanyAccept;
                        memberItem.isMemberAccept = candidate.isMemberAccept;
                        memberItem.JoinedAt = candidate.JoinedAt;
                        memberItem.User = x.User;
                        memberItem.Company = x.Company;
                        data.addMember(memberItem);
                    }
                    else
                    {
                        data.addMember(x);
                    }
                }
            }
            return data;
        }
    }
}
