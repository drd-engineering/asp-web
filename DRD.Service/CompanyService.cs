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
        public long AcceptMember(long memberId)
        {
            using (var db = new ServiceContext())
            {
                Member memberSearch = db.Members.Where(memberItem => memberItem.Id == memberId).FirstOrDefault();
                if (memberSearch != null)
                {
                    memberSearch.isCompanyAccept = true;
                    db.SaveChanges();
                    return memberSearch.UserId;
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
                Member admin = db.Members.Where(member => member.UserId == userId && member.CompanyId == companyId && member.IsAdministrator).FirstOrDefault();
                if (admin == null)
                {
                    // user editting member is not administrator
                    retVal.Add(new AddMemberResponse("", 0, "", -1, ""));
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
                            retVal.Add(new AddMemberResponse(email, 0, "", 0, companyInviting.Name));
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
                                retVal.Add(new AddMemberResponse(email, memberBaru.Id, target.Name, 1, companyInviting.Name));
                            }
                            else
                            {
                                if (lama.isCompanyAccept)
                                {
                                    if (lama.isMemberAccept) retVal.Add(new AddMemberResponse(email, lama.Id, target.Name, -2, companyInviting.Name));
                                    else
                                    {
                                        retVal.Add(new AddMemberResponse(email, lama.Id, target.Name, 2, companyInviting.Name));
                                    }
                                }
                                else
                                {
                                    lama.isCompanyAccept = true;
                                    db.SaveChanges();
                                    retVal.Add(new AddMemberResponse(email, lama.Id, target.Name, -2, companyInviting.Name));
                                }
                            }
                        }
                    }
                }
            }
            return retVal;
        }

        public long ChangeOwner(long companyId, long newOwnerUserId)
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

        public bool CheckIsOwner(long userId, long companyId)
        {
            using (var db = new ServiceContext())
            {
                var owner = db.Companies.Where(memberItem => memberItem.Id == companyId && memberItem.OwnerId == userId).FirstOrDefault();
                return owner == null ? false : true;
            }
        }

        public long ChooseSubscription(int subscriptionId, long companyId)
        {
            using (var db = new ServiceContext())
            {

                Usage Usage = new Usage();
                SubscriptionService subscriptionService = new SubscriptionService();

                Company company = GetCompany(companyId);
                Usage lastSubscription = subscriptionService.getCompanyUsageById(subscriptionId);
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

                long result = db.Usages.Add(Usage).Id;
                db.Usages.Add(lastSubscription);
                db.SaveChanges();

                return result;
            }
        }

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

        public CompanyList GetAllCompanyDetails(long userId)
        {

            using (var db = new ServiceContext())
            {
                var ownerCompanies = db.Companies.Where(companyItem => companyItem.OwnerId == userId && companyItem.IsActive).ToList();
                var listReturn = new CompanyList();
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
                        Usage usage = db.Usages.Where(y => y.Id == subscription.Id && y.IsActive).FirstOrDefault();
                        company.SubscriptionName = db.BusinessPackages.Where(package => package.Id == usage.PackageId).Select(i => i.Name).FirstOrDefault();
                    }
                    company.IsActive = x.IsActive;
                    company.IsVerified = x.IsVerified;
                    company.IsOwnedByUser = (x.Id == userId);
                    company.Administrators = memberService.getAdministrators(company.Id);

                    listReturn.addCompany(company);
                }
                CompanyList companyAsAdmins = GetCompanyListByAdminId(userId);
                listReturn.mergeCompanyList(companyAsAdmins);
                return listReturn;
            }
        }

        public CompanyList GetAllCompanyOwnedbyUser(long userId)
        {
            using (var db = new ServiceContext())
            {
                var result = db.Companies.Where(companyItem => companyItem.OwnerId == userId && companyItem.IsActive).ToList();
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
                    foreach (Company x in result)
                    {
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
                            Usage usage = db.Usages.Where(y => y.Id == subscription.Id && y.IsActive).FirstOrDefault();
                            company.SubscriptionName = db.BusinessPackages.Where(package => package.Id == usage.PackageId).Select(i => i.Name).FirstOrDefault();
                        }
                        company.IsActive = x.IsActive;
                        company.IsVerified = x.IsVerified;
                        company.Administrators = memberService.getAdministrators(company.Id);
                    }
                    return company;
                }
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
        public CompanyList GetCompanyListByAdminId(long adminId)
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
                        company.IsManagedByUser = true;
                        companies.addCompany(company);
                    }
                }
                return companies;
            }
        }

        public CompanyList GetCompanyListByOwnerId(long ownerId)
        {

            using (var db = new ServiceContext())
            {
                var companies = new CompanyList();
                var companyThatOwnedBy = db.Companies.Where(companyItem => companyItem.OwnerId == ownerId && companyItem.IsActive).ToList();
                if (companyThatOwnedBy != null)
                {
                    foreach (Company x in companyThatOwnedBy)
                    {
                        CompanyItem company = new CompanyItem();
                        var subscription = subscriptionService.GetCompanyUsage(x.Id);
                        company.Id = x.Id;
                        company.Code = x.Code;
                        company.Name = x.Name;
                        company.Phone = x.Phone;
                        company.Address = x.Address;
                        company.PointLocation = x.PointLocation;
                        company.OwnerId = x.OwnerId;
                        company.OwnerName = userService.GetName(x.OwnerId);
                        if (subscription != null)
                        {
                            company.SubscriptionId = subscription.Id;
                            Usage usage = db.Usages.Where(y => y.Id == subscription.Id && y.IsActive).FirstOrDefault();
                            company.SubscriptionName = db.BusinessPackages.Where(package => package.Id == usage.PackageId).Select(i => i.Name).FirstOrDefault();
                        }
                        company.IsActive = x.IsActive;
                        company.IsVerified = x.IsVerified;

                        companies.addCompany(company);
                    }
                }
                return companies;
            }
        }

        // POST/GET RejectMember/memberId
        // return user id if member accepted, return -1 if member not found.
        public long RejectMember(long memberId)
        {
            using (var db = new ServiceContext())
            {
                Member memberSearch = db.Members.Where(memberItem => memberItem.Id == memberId).FirstOrDefault();
                if (memberSearch != null)
                {
                    memberSearch.IsActive = false;
                    db.SaveChanges();
                    return memberSearch.UserId;
                }
                else
                    return -1;
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

                body = body.Replace("//images", "/images");

                var senderEmail = configGenerator.GetConstant("EMAIL_USER")["value"];

                var task = emailService.Send(senderEmail, senderName , item.email, senderName + "Member Invitation", body, false, new string[] { });
            }
            // belum register jadi pengguna jadi ya invite aja.
            else if (item.status == 0)
            {
                string body = emailService.CreateHtmlBody(System.Web.HttpContext.Current.Server.MapPath("/doc/emailtemplate/JoinDRD.html"));
                String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
                String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

                body = body.Replace("{_URL_}", strUrl);
                body = body.Replace("{_COMPANYNAME_}", item.companyName);

                body = body.Replace("//images", "/images");

                var senderEmail = configGenerator.GetConstant("EMAIL_USER")["value"];

                var task = emailService.Send(senderEmail, senderName , item.email, senderName + "DRD Invitation", body, false, new string[] { });
            }
        }

        public long VerifyCompany(long companyId)
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
    }
}