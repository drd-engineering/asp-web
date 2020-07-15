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
    public class MemberService
    {
        private CompanyService companyService;
        private ContactService contactService;
        private SubscriptionService subscriptionService ;
        /// <summary>
        /// CHECK if id member already exist or not
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CheckIdIsExist(long id)
        {
            using var db = new Connection();
            return db.Members.Any(i => i.Id == id);
        }
        public Member getMember(long memberId)
        {
            using var db = new Connection();
            return db.Members.Where(memberItem => memberItem.Id == memberId).FirstOrDefault();
        }

        public Member GetMember(long userId, long companyId)
        {
            using var db = new Connection();
            return db.Members.Where(memberItem => memberItem.UserId == userId && memberItem.CompanyId == companyId).FirstOrDefault();
        }
        private void MappingMember(List<Member> membersFromDb, MemberList listReturn)
        {
            companyService = new CompanyService();
            contactService = new ContactService();

            foreach (Member x in membersFromDb)
            {

                var memberItem = new MemberItem
                {
                    Id = x.Id,
                    CompanyId = x.CompanyId,
                    UserId = x.UserId
                };
                memberItem.Company = companyService.GetCompany(memberItem.CompanyId);
                memberItem.User = contactService.getContact(memberItem.UserId);
                memberItem.User.EncryptedId = Utilities.Encrypt(memberItem.UserId.ToString());

                listReturn.addMember(memberItem);
            }
        }
        public MemberList GetAcceptedMember(long companyId)
        {
            using var db = new Connection();
            var listReturn = new MemberList();
            var membersFromDb = db.Members.Where(memberItem => memberItem.CompanyId == companyId && memberItem.IsCompanyAccept && memberItem.IsMemberAccept && memberItem.IsActive).ToList();

            MappingMember(membersFromDb, listReturn);
            return listReturn;
        }
        public MemberList GetWaitingMember(long companyId)
        {
            using var db = new Connection();
            var listReturn = new MemberList(); ;

            var membersFromDb = db.Members.Where(memberItem => memberItem.CompanyId == companyId && !memberItem.IsCompanyAccept && memberItem.IsMemberAccept && memberItem.IsActive).ToList();

            MappingMember(membersFromDb, listReturn);
            return listReturn;
        }

        public MemberList GetAcceptedMember(long companyId, bool isAdmin)
        {
            using (var db = new Connection())
            {
                var listReturn = new MemberList(); ;
                var membersFromDb = db.Members.Where(memberItem => memberItem.CompanyId == companyId && memberItem.IsCompanyAccept && memberItem.IsMemberAccept && memberItem.IsActive && memberItem.IsAdministrator == isAdmin).ToList();

                MappingMember(membersFromDb, listReturn);
                return listReturn;
            }
        }

        public bool CheckIsAdmin(long userId, long companyId)
        {
            using var db = new Connection();
            return db.Members.Any(memberItem => memberItem.CompanyId == companyId && memberItem.IsAdministrator && memberItem.UserId == userId);
        }

        public bool CheckIsOwner(long userId, long companyId)
        {
            using var db = new Connection();
            return db.Companies.Any(memberItem => memberItem.Id == companyId && memberItem.OwnerId == userId);
        }

        public List<MemberItem> GetAdministrators(long CompanyId)
        {
            using var db = new Connection();
            var admins = (from Member in db.Members
                          join User in db.Users on Member.UserId equals User.Id
                          where Member.CompanyId == CompanyId && Member.IsCompanyAccept && Member.IsMemberAccept && Member.IsActive && Member.IsAdministrator
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
        }
        /// <summary>
        /// Obtain all the user data as a member who managed a company or more
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public ICollection<Member> GetAllAdminDataofUser(long adminId)
        {
            using var db = new Connection();
            return db.Members.Where(memberItem => memberItem.UserId == adminId && memberItem.IsActive & memberItem.IsAdministrator).ToList();
        }

        public long Delete(long memberId)
        {
            using var db = new Connection();
            Member member = db.Members.Where(memberItem => memberItem.Id == memberId).FirstOrDefault();
            member.IsActive = false;
            db.SaveChanges();

            return member.Id;
        }
        /// <summary>
        /// SAVE member data requested by company to a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public long AddMemberToCompany(long userId, long companyId)
        {
            using var db = new Connection();
            var member = new Member(userId, companyId, false, true);
            while (CheckIdIsExist(member.Id))
                member.Id = Utilities.RandomLongGenerator(minimumValue: Constant.MINIMUM_VALUE_ID, maximumValue: Constant.MAXIMUM_VALUE_ID);

            db.Members.Add(member);
            db.SaveChanges();
            return member.Id;
        }
        /// <summary>
        /// SAVE member data requested by user who want to join a company
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public long AddMemberRequestToJoinCompany(long userId, long companyId)
        {
            using var db = new Connection();
            var member = new Member(userId, companyId, true, false);
            while (CheckIdIsExist(member.Id))
                member.Id = Utilities.RandomLongGenerator(minimumValue: Constant.MINIMUM_VALUE_ID, maximumValue: Constant.MAXIMUM_VALUE_ID);

            db.Members.Add(member);
            db.SaveChanges();
            return member.Id;
        }
        /// <summary>
        /// SAVE multiple member data requested by company to a user
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="userId"></param>
        /// <param name="emails"></param>
        /// <returns></returns>
        public List<AddMemberResponse> AddMembers(long companyId, long userId, string emails, long loginUserId)
        {
            List<AddMemberResponse> returnValue = new List<AddMemberResponse>();
            string[] listOfEmail = emails.Split(',');
            using var db = new Connection();
            if (!CheckIsAdmin(userId, companyId) && !CheckIsOwner(userId, companyId))
            {
                // user editting member is not administrator or owner
                returnValue.Add(new AddMemberResponse("", 0, "", -1, ""));
            }
            else
            {
                companyService = new CompanyService();
                SmallCompanyData companyInviting = companyService.GetCompany(companyId);
                foreach (var emailItem in listOfEmail)
                {
                    var email = emailItem.Replace(" ", string.Empty);
                    User target = db.Users.Where(user => user.Email.Equals(email)).FirstOrDefault();
                    if (target == null)
                    {
                        // user that wanted to invite is not found (not registered)
                        returnValue.Add(new AddMemberResponse(email, 0, "", 0, companyInviting.Name));
                        continue;
                    }

                    Member existingMember = db.Members.Where(member => member.UserId == target.Id
                        && member.IsActive && member.CompanyId == companyId).FirstOrDefault();
                    //check member exist or not
                    if (existingMember == null)
                    {
                        long memberId = AddMemberToCompany(target.Id, companyInviting.Id);
                        returnValue.Add(new AddMemberResponse(email, memberId, target.Name, 1, companyInviting.Name));
                        continue;
                    }
                    //exist but company hasn't accepeted yet
                    if (!existingMember.IsCompanyAccept)
                    {
                        AuditTrailService.RecordLog(loginUserId, Constant.AuditTrail.Company.ToString(), AuditTrailMessages.AcceptMember(existingMember.UserId, existingMember.CompanyId));

                        existingMember.IsCompanyAccept = true;
                        existingMember.JoinedAt = DateTime.Now;
                        db.SaveChanges();
                        returnValue.Add(new AddMemberResponse(email, existingMember.Id, target.Name, -2, companyInviting.Name));
                        continue;
                    }
                    //exist and already accepted by both side
                    if (existingMember.IsMemberAccept)
                    {
                        returnValue.Add(new AddMemberResponse(email, existingMember.Id, target.Name, -2, companyInviting.Name));
                        continue;
                    }
                    //exist and not accepted yet by user

                    AuditTrailService.RecordLog(loginUserId, Constant.AuditTrail.Company.ToString(), AuditTrailMessages.InviteMember(existingMember.UserId, existingMember.CompanyId));

                    returnValue.Add(new AddMemberResponse(email, existingMember.Id, target.Name, 2, companyInviting.Name));
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Email sender that will sending email to member about their status adding by company, 
        /// this function will not return any response and running in background
        /// Need improvement in how it will handling errors
        /// </summary>
        /// <param name="item"></param>
        public void SendEmailAddMember(AddMemberResponse item)
        {
            var configGenerator = new AppConfigGenerator();
            var topaz = configGenerator.GetConstant("APPLICATION_NAME")["value"];
            var senderName = configGenerator.GetConstant("EMAIL_USER_DISPLAY")["value"];
            EmailService emailService = new EmailService();

            if (item.Status == 1 || item.Status == 2)
            {
                string body = emailService.CreateHtmlBody(System.Web.HttpContext.Current.Server.MapPath("/doc/emailtemplate/MemberInvitation.html"));
                String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
                String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

                body = body.Replace("{_URL_}", strUrl);
                body = body.Replace("{_COMPANYNAME_}", item.CompanyName);
                body = body.Replace("{_NAME_}", item.UserName);
                body = body.Replace("{_MEMBERID_}", item.MemberId.ToString());

                var senderEmail = configGenerator.GetConstant("EMAIL_USER")["value"];

                var task = emailService.Send(senderEmail, senderName, item.Email, senderName + "Member Invitation", body, false, new string[] { });
            }
            // belum register jadi pengguna jadi ya invite aja.
            else if (item.Status == 0)
            {
                string body = emailService.CreateHtmlBody(System.Web.HttpContext.Current.Server.MapPath("/doc/emailtemplate/JoinDRD.html"));
                String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
                String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

                body = body.Replace("{_URL_}", strUrl);
                body = body.Replace("{_COMPANYNAME_}", item.CompanyName);

                var senderEmail = configGenerator.GetConstant("EMAIL_USER")["value"];

                var task = emailService.Send(senderEmail, senderName, item.Email, senderName + " Invitation", body, false, new string[] { });
            }
        }

        /// <summary>
        /// Search some member data based on search query as many as one page requested
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public ICollection<MemberData> GetMembers(long userId, string topCriteria, int page, int pageSize, Expression<Func<MemberData, string>> order)
        {
            Expression<Func<MemberData, bool>> criteriaUsed = WorkflowData => true;
            return FindMembers(userId, topCriteria, page, pageSize, order, criteriaUsed);
        }
        public ICollection<MemberData> FindMembers(long userId, string topCriteria, int page, int pageSize, Expression<Func<MemberData, string>> order, Expression<Func<MemberData, bool>> criteria)
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

            using (var db = new Connection())
            {
                var contactListAllMatch = (from Contact in db.Contacts
                                           join User in db.Users on Contact.ContactItemId equals User.Id
                                           where Contact.ContactOwner.Id == userId && (topCriteria.Equals("") || tops.All(x => (User.Name + " " + User.Phone + " " + User.Email).ToLower().Contains(x.ToLower())))
                                           select new MemberData
                                           {
                                               Id = User.Id,
                                               Name = User.Name,
                                               Phone = User.Phone,
                                               Email = User.Email,
                                               ImageProfile = User.ProfileImageFileName
                                           }).Union(from member1 in db.Members
                                                    join company in db.Companies on member1.CompanyId equals company.Id
                                                    join member2 in db.Members on company.Id equals member2.CompanyId
                                                    join user in db.Users on member2.UserId equals user.Id
                                                    where member1.UserId == userId
                                                    && member1.IsActive && member1.IsCompanyAccept && member1.IsMemberAccept
                                                    && member2.IsActive && member2.IsCompanyAccept && member2.IsMemberAccept
                                                    && (topCriteria.Equals("") || tops.All(x => (user.Name + " " + user.Phone + " " + user.Email).ToLower().Contains(x.ToLower())))
                                                    select new MemberData
                                                    {
                                                        Id = user.Id,
                                                        Name = user.Name,
                                                        Phone = user.Phone,
                                                        Email = user.Email,
                                                        ImageProfile = user.ProfileImageFileName
                                                    }).Where(criteria).OrderBy(member => member.Name).Skip(skip).Take(pageSize).ToList();
                if (contactListAllMatch != null)
                    for (var i = 0; i < contactListAllMatch.Count(); i++)
                    {
                        var item = contactListAllMatch.ElementAt(i);
                        item.EncryptedId = Utilities.Encrypt(item.Id.ToString());
                        contactListAllMatch[i] = item;
                    }

                return contactListAllMatch;
            }
        }
        /// <summary>
        /// Find how many userd that is related to the query
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="topCriteria"></param>
        /// <returns></returns>
        public int CountMembers(long userId, string topCriteria)
        {
            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = "";

            using (var db = new Connection())
            {
                var countContactListAllMatch = (from Contact in db.Contacts
                                                join User in db.Users on Contact.ContactItemId equals User.Id
                                                where Contact.ContactOwner.Id == userId && (topCriteria.Equals("") || tops.All(x => (User.Name + " " + User.Phone + " " + User.Email).ToLower().Contains(x.ToLower())))
                                                select new MemberData
                                                {
                                                    Id = User.Id,
                                                    Name = User.Name,
                                                    Phone = User.Phone,
                                                    Email = User.Email,
                                                    ImageProfile = User.ProfileImageFileName
                                                }).Union(from member1 in db.Members
                                                         join company in db.Companies on member1.CompanyId equals company.Id
                                                         join member2 in db.Members on company.Id equals member2.CompanyId
                                                         join user in db.Users on member2.UserId equals user.Id
                                                         where member1.UserId == userId
                                                         && member1.IsActive && member1.IsCompanyAccept && member1.IsMemberAccept
                                                         && member2.IsActive && member2.IsCompanyAccept && member2.IsMemberAccept
                                                         && (topCriteria.Equals("") || tops.All(x => (user.Name + " " + user.Phone + " " + user.Email).ToLower().Contains(x.ToLower())))
                                                         select new MemberData
                                                         {
                                                             Id = user.Id,
                                                             Name = user.Name,
                                                             Phone = user.Phone,
                                                             Email = user.Email,
                                                             ImageProfile = user.ProfileImageFileName
                                                         }).Count();
                return countContactListAllMatch;
            }
        }
        /// <summary>
        /// Search some members that is participate in rotation that related to query requested
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="rotationId"></param>
        /// <returns></returns>
        public ICollection<MemberData> FindMembersRotation(long userId, string topCriteria, int page, int pageSize, long rotationId)
        {
            Expression<Func<MemberData, bool>> criteria = MemberData => true;
            int skip = pageSize * (page - 1);
            Expression<Func<MemberData, string>> ordering = MemberData => "Name";
            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = "";

            using (var db = new Connection())
            {
                var contactListAllMatch = (from RotationUser in db.RotationUsers
                                           join User in db.Users on RotationUser.UserId equals User.Id
                                           where User.Id != userId && RotationUser.RotationId == rotationId
                                           && (topCriteria.Equals("") || tops.All(x => (User.Name + " " + User.Phone + " " + User.Email).ToLower().Contains(x.ToLower())))
                                           select new MemberData
                                           {
                                               Id = User.Id,
                                               Name = User.Name,
                                               Phone = User.Phone,
                                               Email = User.Email,
                                               ImageProfile = User.ProfileImageFileName
                                           }).Where(criteria).OrderBy(member => member.Name).Skip(skip).Take(pageSize).ToList();
                if (contactListAllMatch != null)
                    for (var i = 0; i < contactListAllMatch.Count(); i++)
                    {
                        var item = contactListAllMatch.ElementAt(i);
                        item.EncryptedId = Utilities.Encrypt(item.Id.ToString());
                        contactListAllMatch[i] = item;
                    }
                return contactListAllMatch;
            }
        }
        /// <summary>
        /// Find how many User that participate in the Rotation requested
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="topCriteria"></param>
        /// <param name="rotationId"></param>
        /// <returns></returns>
        public int FindMembersRotationCountAll(long userId, string topCriteria, long rotationId)
        {
            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = "";

            using (var db = new Connection())
            {
                var countContactListAllMatch = (from RotationUser in db.RotationUsers
                                                join User in db.Users on RotationUser.UserId equals User.Id
                                                where User.Id != userId && RotationUser.RotationId == rotationId
                                                && (topCriteria.Equals("") || tops.All(x => (User.Name + " " + User.Phone + " " + User.Email).ToLower().Contains(x.ToLower())))
                                                select new MemberData
                                                {
                                                    Id = User.Id,
                                                    Name = User.Name,
                                                    Phone = User.Phone,
                                                    Email = User.Email,
                                                    ImageProfile = User.ProfileImageFileName
                                                }).Count();
                return countContactListAllMatch;
            }
        }
        /// <summary>
        /// Accepting Invitation from company to join the company as a member, return true if success return false if failed/invalid
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public bool AcceptInvitation(long userId, long memberId)
        {
            using (var db = new Connection())
            {
                Member theUser = db.Members.FirstOrDefault(m => m.Id == memberId && m.UserId == userId && m.IsCompanyAccept && !m.IsMemberAccept);
                if (theUser != null)
                {
                    theUser.IsMemberAccept = true;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }
        public MemberList BecomeAdmin(long companyId, ICollection<MemberItem> adminCandidate)
        {
            subscriptionService = new SubscriptionService();
            MemberList data = new MemberList();
            int totalAdmin = 0;

            if (adminCandidate!=null)
                totalAdmin = adminCandidate.Count;

            //check admin limit
            var subscriptionStatus = subscriptionService.CheckOrAddSpecificUsage(Models.Constant.BusinessPackageItem.Administrator, companyId, totalAdmin, addAfterSubscriptionValid: true, reset: true);
            data.status = subscriptionStatus.ToString();
            if (!subscriptionStatus.Equals(Constant.BusinessUsageStatus.OK) || adminCandidate==null)
                return data;

            using (var db = new Connection())
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
                        memberItem.isCompanyAccept = candidate.IsCompanyAccept;
                        memberItem.isMemberAccept = candidate.IsMemberAccept;
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
            using (var db = new Connection())
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
                        memberItem.isCompanyAccept = candidate.IsCompanyAccept;
                        memberItem.isMemberAccept = candidate.IsMemberAccept;
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
