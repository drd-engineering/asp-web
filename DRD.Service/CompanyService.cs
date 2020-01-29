using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Models;
using DRD.Models.API;
using DRD.Service.Context;
namespace DRD.Service
{
    public class CompanyService
    {
        private MemberService memberService;
        private UserService userService;
        private SubscriptionService subscriptionService;

        public CompanyList GetAllCompany()
        {
            using (var db = new ServiceContext())
            {
                var result = db.Companies.Where(companyItem => companyItem.IsActive).ToList();
                var listReturn = new CompanyList();
                foreach (Company x in result)
                {
                    var company = new CompanyItem();
                    company.Id = x.Id;
                    company.Code = x.Code;
                    company.Name = x.Name;
                    listReturn.companies.Add(company);
                }
                return listReturn;
            }
        }

        public CompanyItem GetCompanyItem(long companyId)
        {
            using (var db = new ServiceContext())
            {
                var result = (from Company in db.Companies
                              where Company.Id == companyId
                              select new CompanyItem
                              {
                                  Id = Company.Id,
                                  Name = Company.Name,
                                  Phone = Company.Phone,
                                  Email = Company.Email
                              }
                            ).FirstOrDefault();
                return result;
            }
        }

        public bool checkIsOwner(long userId, long companyId)
        {
            using (var db = new ServiceContext())
            {
                var owner = db.Companies.Where(memberItem => memberItem.Id == companyId && memberItem.OwnerId == userId).FirstOrDefault();
                return owner == null ? false : true;
            }
        }

        public PlanBusiness getCompanySubscription(long companyId)
        {
            using (var db = new ServiceContext())
            {
                if (db.PlanBusinesses != null)
                {
                    var subscription = db.PlanBusinesses.Where(subs => subs.CompanyId == companyId && subs.IsActive).FirstOrDefault();
                    return subscription;
                }
                return null;
            }
        }
        public CompanyList getCompanyListByAdminId(long adminId)
        {
            memberService = new MemberService();
            using (var db = new ServiceContext())
            {
                var companies = new CompanyList();
                var companyAdmins = memberService.getMemberByCompanyAdmin(adminId);
                foreach (Member x in companyAdmins)
                {
                    var company = GetCompanyDetail(x.CompanyId);
                    if (company != null)
                    {
                        companies.addCompany(company);
                    }
                }
                return companies;
            }
        }

        public CompanyList GetAllCompanyDetails(long userId)
        {
            memberService = new MemberService();
            subscriptionService = new SubscriptionService();
            userService = new UserService();

            using (var db = new ServiceContext())
            {

                var ownerCompanies = db.Companies.Where(companyItem => companyItem.OwnerId==userId && companyItem.IsActive).ToList();
                var listReturn = new CompanyList();
                System.Diagnostics.Debug.WriteLine("TES OWNER COMPANIES  :: " + ownerCompanies );
                foreach (Company x in ownerCompanies)
                {
                    var company = new CompanyItem();
                System.Diagnostics.Debug.WriteLine("TES OWNER COMPANIES id  :: " + x.Id );
                    var subscription = getCompanySubscription(x.Id);
                    company.Id = x.Id;
                    company.Code = x.Code;
                    company.Name = x.Name;
                    company.Phone = x.Phone;
                    company.Address = x.Address;
                    company.PointLocation = x.PointLocation;
                    company.OwnerId = x.OwnerId;
                    company.OwnerName = userService.GetName(company.OwnerId);
                    if (subscription != null) { company.SubscriptionId = subscription.Id; }
                    if (subscription != null) { company.SubscriptionName = subscription.SubscriptionName; }
                    company.IsActive = x.IsActive;
                    company.IsVerified = x.IsVerified;
                    company.Administrators = memberService.getAdministrators(company.Id);
                    System.Diagnostics.Debug.WriteLine("TES OWNER COMPANIES LIST INSIDE LOOP :: " + company.Id);

                    listReturn.addCompany(company);
                }
                CompanyList companyAsAdmins = getCompanyListByAdminId(userId);
                listReturn.mergeCompanyList(companyAsAdmins);
                System.Diagnostics.Debug.WriteLine("TES OWNER COMPANIES LIST :: " + listReturn.companies.Count);
                return listReturn;
            }
        }
        public CompanyItem GetCompanyDetail(long id)
        {
            memberService = new MemberService();

            using (var db = new ServiceContext())
            {
                var result = db.Companies.Where(companyItem => companyItem.Id == id).ToList();
                if (result.Count == 0)
                {
                    return null;
                }
                else
                {
                    var company = new CompanyItem();
                    foreach (Company x in result)
                    {
                        var subscription = getCompanySubscription(x.Id);
                        company.Id = x.Id;
                        company.Code = x.Code;
                        company.Name = x.Name;
                        company.Phone = x.Phone;
                        company.Address = x.Address;
                        company.PointLocation = x.PointLocation;
                        company.OwnerId = x.OwnerId;
                        company.OwnerName = userService.GetName(company.OwnerId);
                        if (subscription != null) { company.SubscriptionId = subscription.Id; }
                        if (subscription != null) { company.SubscriptionName = subscription.SubscriptionName; }
                        company.IsActive = x.IsActive;
                        company.IsVerified = x.IsVerified;
                        company.Administrators = memberService.getAdministrators(company.Id);
                    }
                    return company;
                }
            }
        }

        public CompanyItem GetCompany(int id)
        {
            using (var db = new ServiceContext())
            {
                var result = db.Companies.Where(companyItem => companyItem.Id == id).ToList();
                if (result.Count == 0)
                {
                    return null;
                }
                else
                {
                    var company = new CompanyItem();
                    foreach(Company x in result){
                        company.Id = x.Id;
                        company.Name = x.Name;
                        company.Code = x.Code;
                    }
                    return company;
                }
            }
        }

        public Company GetCompany(long id)
        {
            using (var db = new ServiceContext())
            {
                return db.Companies.Where(companyItem => companyItem.Id == id).FirstOrDefault();
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

                        //TODO: remove these lines when production
                        System.Diagnostics.Debug.WriteLine("[[USERSERVICE]]User ID expected when saving : " + company.Id);

                        
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

        public long changeOwner(long companyId, long newOwnerUserId)
        {
            using (var db = new ServiceContext())
            {
                Company company = GetCompany(companyId);
                company.OwnerId = newOwnerUserId;

                long result = db.Companies.Add(company).OwnerId;
                db.SaveChanges();

                return result;
            }
        }

        public long chooseSubscription(int subscriptionId, long companyId)
        {
            using (var db = new ServiceContext())
            {
                CompanyQuota companyQuota = new CompanyQuota();
                SubscriptionDictionary subscriptionDictionary = new SubscriptionDictionary();

                Company company = GetCompany(companyId);
                CompanyQuota lastSubscription = getLastSubscription(companyId);
                Subscription subscription = subscriptionDictionary.getBusinessSubscription[subscriptionId];

                if (company != null && subscription != null)
                {
                    companyQuota.CompanyId = company.Id;
                    companyQuota.startedAt = DateTime.Now;
                    companyQuota.expiredAt = DateTime.Now.AddDays(subscription.DurationInDays);
                    companyQuota.AdministratorQuota = subscription.AdministratorQuota;
                    companyQuota.StorageQuota = subscription.StorageQuotaInByte;
                    companyQuota.BusinessSubscriptionId = subscriptionId;
                    companyQuota.StorageUsage = lastSubscription == null ? 0 : lastSubscription.StorageUsage;
                    companyQuota.isActive = true;
                    lastSubscription.isActive = false;
                }


                long result = db.CompanyQuotas.Add(companyQuota).Id;
                long lastResult = db.CompanyQuotas.Add(lastSubscription).Id;
                db.SaveChanges();

                return result;
            }
        }

        public CompanyQuota getLastSubscription(long companyId)
        {
            using (var db = new ServiceContext())
            {
                return db.CompanyQuotas.Where(companyItem => companyItem.CompanyId == companyId).LastOrDefault();
            }
        }
        public CompanyQuota getActiveSubscription(long companyId)
        {
            using (var db = new ServiceContext())
            {
                return db.CompanyQuotas.Where(companyItem => companyItem.CompanyId == companyId && companyItem.isActive).FirstOrDefault();
            }
        }

        public long verifyCompany(long companyId)
        {
            using (var db = new ServiceContext())
            {
                Company company = GetCompany(companyId);
                company.IsVerified = true;

                long result = db.Companies.Add(company).OwnerId;
                db.SaveChanges();

                return result;
            }
        }

        public List<AddMemberResponse> AddMembers(long companyId, long userId, string emails)
        {
            List<AddMemberResponse> retVal = new List<AddMemberResponse>();
            string[] listOfEmail = emails.Split(',');
            using (var db = new ServiceContext())
            {
                Member admin = db.Members.Where(member => member.UserId == userId && member.CompanyId == companyId && member.IsAdministrator).FirstOrDefault();
                if (admin == null)
                {
                    // user editting member is not administrator
                    retVal.Add(new AddMemberResponse("", -1));
                }
                else
                {
                    Company companyInviting = db.Companies.Where(company => company.Id == companyId).FirstOrDefault();
                    foreach (var emailItem in listOfEmail)
                    {
                        var email = emailItem.Replace(" ", string.Empty);
                        Member memberBaru = new Member();
                        User target = db.Users.Where(user => user.Email.Equals(email)).FirstOrDefault();
                        if (target == null)
                        {
                            // user that wanted to invite is not found (not registered)
                            retVal.Add(new AddMemberResponse(email, 0));
                        }
                        else
                        {
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
                                retVal.Add(new AddMemberResponse(email, 1));
                            }
                            else
                            {
                                // member already added
                                retVal.Add(new AddMemberResponse(email, -2));
                            }
                        }
                    }
                }
            }
            return retVal;
        }

        public void sendEmailAddMember(string email, int status, string company)
        {
            //TODO: remove these lines when production
            System.Diagnostics.Debug.WriteLine("[[USERSERVICE]]Send Email Trigered");
            var configGenerator = new AppConfigGenerator();
            var topaz = configGenerator.GetConstant("APPLICATION_NAME")["value"];
            var senderName = configGenerator.GetConstant("EMAIL_USER_DISPLAY")["value"];
            EmailService emailService = new EmailService();
            
            if(status == 1)
            {
                string body = emailService.CreateHtmlBody(System.Web.HttpContext.Current.Server.MapPath("/doc/emailtemplate/??.html"));
                String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
                String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

                //TODO: remove these lines when production
                System.Diagnostics.Debug.WriteLine("[[USERSERVICE]]This is the pathquery of Email Registration " + strPathAndQuery);

                body = body.Replace("{_URL_}", strUrl);
                body = body.Replace("{_COMPANYNAME_}", company);

                body = body.Replace("//images", "/images");

                var senderEmail = configGenerator.GetConstant("EMAIL_USER")["value"];

                //TODO: remove these lines when production
                System.Diagnostics.Debug.WriteLine("[[USERSERVICE]]This is the sender of Email Registration " + senderEmail);

                var task = emailService.Send(senderEmail, senderName + " Administrator", email, senderName + " User Registration", body, false, new string[] { });
            }
            else if (status == 0)
            {
                string body = emailService.CreateHtmlBody(System.Web.HttpContext.Current.Server.MapPath("/doc/emailtemplate/??.html"));
                String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
                String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

                //TODO: remove these lines when production
                System.Diagnostics.Debug.WriteLine("[[USERSERVICE]]This is the pathquery of Email Registration " + strPathAndQuery);

                body = body.Replace("{_URL_}", strUrl);
                body = body.Replace("{_COMPANYNAME_}", company);

                body = body.Replace("//images", "/images");

                var senderEmail = configGenerator.GetConstant("EMAIL_USER")["value"];

                //TODO: remove these lines when production
                System.Diagnostics.Debug.WriteLine("[[USERSERVICE]]This is the sender of Email Registration " + senderEmail);

                var task = emailService.Send(senderEmail, senderName + " Administrator", email, senderName + " User Registration", body, false, new string[] { });
            }
        }

        // POST/GET AcceptMember/memberId
        // return id if member accepted, return -1 if member not found. 
        public long AcceptMember(long memberId)
        {
            using (var db = new ServiceContext())
            {
                Member memberSearch = db.Members.Where(memberItem => memberItem.Id == memberId).FirstOrDefault();
                if (memberSearch != null)
                {
                    memberSearch.isCompanyAccept = true;
                    db.SaveChanges();
                    return memberSearch.Id;
                }
                else
                    return -1;
            }
        }
        // POST/GET RejectMember/memberId
        // return id if member accepted, return -1 if member not found. 
        public long RejectMember(long memberId)
        {
            using (var db = new ServiceContext())
            {
                Member memberSearch = db.Members.Where(memberItem => memberItem.Id == memberId).FirstOrDefault();
                if (memberSearch != null)
                {
                    memberSearch.IsActive = false;
                    db.SaveChanges();
                    return memberSearch.Id;
                }
                else
                    return -1;
            }
        }
        
    }
}
