using DRD.Models;
using DRD.Models.API;
using DRD.Service.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace DRD.Service
{
    public class CompanyService
    {
        private MemberService memberService = new MemberService();
        private SubscriptionService subscriptionService = new SubscriptionService();
        private UserService userService = new UserService();
        // POST/GET AcceptMember/memberId
        // return user id if member accepted, return -1 if member not found.
        public int AcceptMember(long memberId)
        {
            using (var db = new ServiceContext())
            {
                Member memberSearch = db.Members.Where(memberItem => memberItem.Id == memberId).FirstOrDefault();
                if (memberSearch != null)
                {
                    memberSearch.isCompanyAccept = true;
                    var subscriptionStatus = subscriptionService.CheckOrAddSpecificUsage(Constant.BusinessPackageItem.Member, memberSearch.CompanyId, 1);
                    if (subscriptionStatus.Equals(Constant.BusinessUsageStatus.OK))
                        db.SaveChanges();
                    return (int) subscriptionStatus;
                }
                else
                    return -1;
            }
        }

        public void AddCompany(Company company)
        {
            using (var db = new ServiceContext())
            {
                var result = db.Companies.Where(companyItem => companyItem.Name.Equals(company.Name)).ToList();
                if (result.Count != 0)
                {
                    return;
                }
                Company newCompany = new Company();
                newCompany.Name = company.Name;
                newCompany.OwnerId = company.OwnerId;
                newCompany.Address = company.Address;
                newCompany.Descr = company.Descr;
                newCompany.Email = company.Email;
                newCompany.Phone = company.Phone;
                newCompany.PointLocation = company.PointLocation;
                newCompany.PostalCode = company.PostalCode;

                CompanyService companyService = new CompanyService();
                long companyId = companyService.Save(company);
            }
        }

        public List<AddMemberResponse> AddMembers(long companyId, long userId, string emails)
        {
            List<AddMemberResponse> retVal = new List<AddMemberResponse>();
            string[] listOfEmail = emails.Split(',');
            using (var db = new ServiceContext())
            {
                if (!memberService.checkIsAdmin(userId, companyId) && !memberService.checkIsOwner(userId, companyId))
                {
                    // user editting member is not administrator or owner
                    retVal.Add(new AddMemberResponse("", 0, "", -1, ""));
                }
                else
                {
                    CompanyService cpserv = new CompanyService();
                    SmallCompanyData companyInviting = cpserv.GetCompany(companyId);
                    foreach (var emailItem in listOfEmail)
                    {
                        var email = emailItem.Replace(" ", string.Empty);
                        Member memberBaru = new Member();
                        User target = db.Users.Where(user => user.Email.Equals(email)).FirstOrDefault();
                        if (target == null)
                        {
                            // user that wanted to invite is not found (not registered)
                            retVal.Add(new AddMemberResponse(email, 0, "", 0, companyInviting.Name));
                            continue;
                        }
                        
                        Member lama = db.Members.Where(member => member.UserId == target.Id
                            && member.IsActive && member.CompanyId == companyId).FirstOrDefault();
                        if (lama == null)
                        {
                            memberBaru.UserId = target.Id;
                            memberBaru.CompanyId = companyId;
                            memberBaru.isCompanyAccept = true;
                            memberBaru.JoinedAt = DateTime.Now;
                            db.Members.Add(memberBaru);
                            db.SaveChanges();
                            // success adding new member
                            retVal.Add(new AddMemberResponse(email, memberBaru.Id, target.Name, 1, companyInviting.Name));
                            continue;
                        }

                        if (!lama.isCompanyAccept)
                        {
                            lama.isCompanyAccept = true;
                            db.SaveChanges();
                            retVal.Add(new AddMemberResponse(email, lama.Id, target.Name, -2, companyInviting.Name));
                            continue;
                        }

                        if (lama.isMemberAccept)
                        {
                            retVal.Add(new AddMemberResponse(email, lama.Id, target.Name, -2, companyInviting.Name));
                            continue;
                        }

                        retVal.Add(new AddMemberResponse(email, lama.Id, target.Name, 2, companyInviting.Name));
                    }
                }
            }
            return retVal;
        }

        public long ChangeOwner(long companyId, long newOwnerUserId)
        {
            using (var db = new ServiceContext())
            {
                Company company = GetCompanyDb(companyId);
                company.OwnerId = newOwnerUserId;

                long result = db.Companies.Add(company).OwnerId;
                db.SaveChanges();

                return result;
            }
        }

        public long ChooseSubscription(int subscriptionId, long companyId)
        {
            using (var db = new ServiceContext())
            {

                BusinessUsage Usage = new BusinessUsage();
                SubscriptionService subscriptionService = new SubscriptionService();

                Company company = GetCompanyDb(companyId);
                BusinessUsage lastSubscription = subscriptionService.getCompanyUsageById(subscriptionId);
                BusinessPackage package = subscriptionService.getCompanyPackageByCompany(companyId);
                Price price = subscriptionService.getActivePricePackage(package.Id);

                if (company != null && package != null && price != null)
                {
                    Usage.CompanyId = company.Id;
                    Usage.StartedAt = DateTime.Now;
                    Usage.ExpiredAt = DateTime.Now.AddDays(package.Duration);
                    Usage.Administrator = 0;
                    Usage.Storage = 0;
                    Usage.PackageId = package.Id;
                    Usage.PriceId = price.Id;
                    Usage.IsActive = true;
                    lastSubscription.IsActive = false;
                    lastSubscription.ExpiredAt = DateTime.Now;
                }

                long result = db.BusinessUsages.Add(Usage).Id;
                db.BusinessUsages.Add(lastSubscription);
                db.SaveChanges();

                return result;
            }
        }

        /// <summary>
        /// Obtain all company available in DRD 
        /// </summary>
        /// <returns>only contains little details like id, name and code</returns>
        public ICollection<SmallCompanyData> GetAllCompany()
        {
            using (var db = new ServiceContext())
            {
                var result = (from cmpny in db.Companies
                              where cmpny.IsActive
                              select new SmallCompanyData
                              {
                                  Id = cmpny.Id,
                                  Code = cmpny.Code,
                                  Name = cmpny.Name,
                              }).ToList();
                return result;
            }
        }
        /// <summary>
        /// Obtain all company that user manage and own
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ICollection<CompanyItem> GetOwnedandManagedCompany(long userId)
        {
            using (var db = new ServiceContext())
            {
                var managedCompany = GetAllCompanyDetailByAdminId(userId);
                var ownedCompany = GetAllCompanyDetailByOwnerId(userId);
                // merge two list of company
                List<CompanyItem> listReturn = new List<CompanyItem>();
                listReturn.AddRange(ownedCompany);
                listReturn.AddRange(managedCompany);
                return listReturn;
            }
        }
        /// <summary>
        /// Obtain all company that user manage
        /// </summary>
        /// <param name="adminId"> user Id as admin</param>
        /// <returns></returns>
        public ICollection<CompanyItem> GetAllCompanyDetailByAdminId(long adminId)
        {
            memberService = new MemberService();
            using (var db = new ServiceContext())
            {
                var companies = new List<CompanyItem>();
                var memberAsAdmins = memberService.GetAllAdminDataofUser(adminId);
                foreach (Member member in memberAsAdmins)
                {
                    var company = GetCompanyDetail(member.CompanyId);
                    if (company == null) continue;
                    company.IsManagedByUser = true;
                    companies.Add(company);
                }
                return companies;
            }
        }
        /// <summary>
        /// Obtain all company that user own
        /// </summary>
        /// <param name="ownerId">user id as owner</param>
        /// <returns></returns>
        public ICollection<CompanyItem> GetAllCompanyDetailByOwnerId(long ownerId)
        {
            using (var db = new ServiceContext())
            {
                var ownerCompanies = db.Companies.Where(companyItem => companyItem.OwnerId == ownerId && companyItem.IsActive).ToList();
                var listReturn = new List<CompanyItem>();
                foreach (Company x in ownerCompanies)
                {
                    var company = new CompanyItem();
                    var subscription = subscriptionService.GetCompanyUsage(x.Id);
                    company.Id = x.Id;
                    company.Code = x.Code;
                    company.Name = x.Name;
                    company.Phone = x.Phone;
                    company.Address = x.Address;
                    company.PointLocation = x.PointLocation;
                    company.OwnerId = x.OwnerId;
                    company.OwnerName = userService.GetName(company.OwnerId);
                    if (subscription != null)
                    {
                        company.SubscriptionId = subscription.Id;
                        BusinessUsage usage = db.BusinessUsages.Where(y => y.Id == subscription.Id && y.IsActive).FirstOrDefault();
                        company.SubscriptionName = db.BusinessPackages.Where(package => package.Id == usage.PackageId).Select(i => i.Name).FirstOrDefault();
                    }
                    company.IsActive = x.IsActive;
                    company.IsVerified = x.IsVerified;
                    company.IsOwnedByUser = true;
                    company.Administrators = memberService.getAdministrators(company.Id);
                    listReturn.Add(company);
                }
                return listReturn;
            }
        }
        /// <summary>
        /// Obtain all company that user has
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>only contain small data like id, code and name of company</returns>
        public ICollection<SmallCompanyData> GetAllCompanyOwnedbyUser(long userId)
        {
            using (var db = new ServiceContext())
            {
                var result = (from cmpny in db.Companies
                              where cmpny.OwnerId == userId && cmpny.IsActive
                              select new SmallCompanyData
                              {
                                  Id = cmpny.Id,
                                  Code = cmpny.Code,
                                  Name = cmpny.Name,
                              }).ToList();
                return result;
            }
        }
        /// <summary>
        /// Obtain company details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CompanyItem GetCompanyDetail(long id)
        {
            using (var db = new ServiceContext())
            {
                var result = db.Companies.Where(companyItem => companyItem.Id == id).FirstOrDefault();
                if (result == null) return null;
                var company = new CompanyItem();
                var subscription = subscriptionService.GetCompanyUsage(result.Id);
                company.Id = result.Id;
                company.Code = result.Code;
                company.Name = result.Name;
                company.Phone = result.Phone;
                company.Address = result.Address;
                company.PointLocation = result.PointLocation;
                company.OwnerId = result.OwnerId;
                company.OwnerName = userService.GetName(company.OwnerId);
                if (subscription != null)
                {
                    company.SubscriptionId = subscription.Id;
                    BusinessUsage usage = db.BusinessUsages.Where(y => y.Id == subscription.Id && y.IsActive).FirstOrDefault();
                    company.SubscriptionName = db.BusinessPackages.Where(package => package.Id == usage.PackageId).Select(i => i.Name).FirstOrDefault();
                }
                company.IsActive = result.IsActive;
                company.IsVerified = result.IsVerified;
                company.Administrators = memberService.getAdministrators(company.Id);
                return company;
            }
        }
        /// <summary>
        /// Obtain company little details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SmallCompanyData GetCompany(long id)
        {
            using (var db = new ServiceContext())
            {
                var result = (from cmpny in db.Companies
                              where cmpny.Id == id && cmpny.IsActive
                              select new SmallCompanyData
                              {
                                  Id = cmpny.Id,
                                  Code = cmpny.Code,
                                  Name = cmpny.Name,
                              }).FirstOrDefault();
                return result;
            }
        }
        /// <summary>
        /// Obtain company data raw from database it's mean one data contain all atribute that company has
        /// </summary>
        /// <param name="id"></param>
        /// <returns>company contain all atribute from db</returns>
        public Company GetCompanyDb(long id)
        {
            using (var db = new ServiceContext())
            {
                return db.Companies.Where(companyItem => companyItem.Id == id).FirstOrDefault();
            }
        }
        /// <summary>
        /// Reject a member request to join the company
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns> return user id if member rejected, return -1 if member not found. </returns>
        public long RejectMember(long memberId)
        {
            using (var db = new ServiceContext())
            {
                Member memberSearch = db.Members.Where(memberItem => memberItem.Id == memberId).FirstOrDefault();
                if (memberSearch == null) return -1;
                memberSearch.IsActive = false;
                memberSearch.isCompanyAccept = false;
                db.SaveChanges();
                return memberSearch.UserId;
            }
        }

        public long Save(Company company)
        {
            int result = 0;
            using (var db = new ServiceContext())
            {
                for (int i = 0; i < Constant.TEST_DUPLICATION_COUNT; i++)
                {
                    try
                    {
                        company.Id = Utilities.RandomLongGenerator(minimumValue: 1000000000, maximumValue: 10000000000);

                        db.Companies.Add(company);
                        result = db.SaveChanges();
                        break;
                    }
                    catch (DbUpdateException x)
                    {
                        if (i > Constant.TEST_DUPLICATION_COUNT)
                            throw new Exception(x.Message);
                    }
                }

                return company.Id;
            }
        }
        public long VerifyCompany(long companyId)
        {
            using (var db = new ServiceContext())
            {
                Company company = GetCompanyDb(companyId);
                company.IsVerified = true;

                long result = db.Companies.Add(company).OwnerId;
                db.SaveChanges();

                return result;
            }
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

            if (item.status == 1 || item.status == 2)
            {
                string body = emailService.CreateHtmlBody(System.Web.HttpContext.Current.Server.MapPath("/doc/emailtemplate/MemberInvitation.html"));
                String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
                String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

                body = body.Replace("{_URL_}", strUrl);
                body = body.Replace("{_COMPANYNAME_}", item.companyName);
                body = body.Replace("{_NAME_}", item.userName);
                body = body.Replace("{_MEMBERID_}", item.memberId.ToString());

                var senderEmail = configGenerator.GetConstant("EMAIL_USER")["value"];

                var task = emailService.Send(senderEmail, senderName, item.email, senderName + "Member Invitation", body, false, new string[] { });
            }
            // belum register jadi pengguna jadi ya invite aja.
            else if (item.status == 0)
            {
                string body = emailService.CreateHtmlBody(System.Web.HttpContext.Current.Server.MapPath("/doc/emailtemplate/JoinDRD.html"));
                String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
                String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

                body = body.Replace("{_URL_}", strUrl);
                body = body.Replace("{_COMPANYNAME_}", item.companyName);

                var senderEmail = configGenerator.GetConstant("EMAIL_USER")["value"];

                var task = emailService.Send(senderEmail, senderName, item.email, senderName + " Invitation", body, false, new string[] { });
            }
        }
    }
}