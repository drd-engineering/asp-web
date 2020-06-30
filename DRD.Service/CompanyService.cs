using DRD.Models;
using DRD.Models.API;
using DRD.Service.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DRD.Service
{
    public class CompanyService
    {
        private MemberService memberService;
        private SubscriptionService subscriptionService = new SubscriptionService();
        private UserService userService = new UserService();


        // POST/GET AcceptMember/memberId
        // return user id if member accepted, return -1 if member not found.
        public string AcceptMember(long memberId)
        {
            using (var db = new ServiceContext())
            {
                Member memberSearch = db.Members.Where(memberItem => memberItem.Id == memberId).FirstOrDefault();
                if (memberSearch != null)
                {
                    memberSearch.IsCompanyAccept = true;
                    var subscriptionStatus = subscriptionService.CheckOrAddSpecificUsage(Constant.BusinessPackageItem.Member, memberSearch.CompanyId, 1, addAfterSubscriptionValid:true);
                    if (subscriptionStatus.Equals(Constant.BusinessUsageStatus.OK))
                        db.SaveChanges();
                    return subscriptionStatus.ToString();
                }
                else
                    return Constant.InivitationStatus.ERROR_NOT_FOUND.ToString();
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
                newCompany.Description = company.Description;
                newCompany.Email = company.Email;
                newCompany.Phone = company.Phone;
                newCompany.MapCoordinate = company.MapCoordinate;
                newCompany.PostalCode = company.PostalCode;

                CompanyService companyService = new CompanyService();
                long companyId = companyService.Save(company);
            }
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
                    Usage.CreatedAt = DateTime.Now;
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
        /// GET all company available in DRD 
        /// </summary>
        /// <returns>only contains little details like id, name and code</returns>
        public ICollection<SmallCompanyData> GetCompanies()
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
        /// GET all company that user manage and own
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
        /// GET all company that user manage
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
        /// GET all company that user own
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
                    company.PointLocation = x.MapCoordinate;
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
        /// GET all company that user has
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>only contain small data like id, code and name of company</returns>
        public ICollection<SmallCompanyData> GetCompaniesOwnedByUser(long userId)
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
        /// GET company details
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
                company.PointLocation = result.MapCoordinate;
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
        /// GET company little details
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
        /// GET company data raw from database it's mean one data contain all atribute that company has
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
        public string RejectMember(long memberId)
        {
            using (var db = new ServiceContext())
            {
                Member memberSearch = db.Members.Where(memberItem => memberItem.Id == memberId).FirstOrDefault();
                //error not found
                if (memberSearch == null) return Constant.InivitationStatus.ERROR_NOT_FOUND.ToString();
                
                //check subscription
                var subscriptionStatus = subscriptionService.CheckOrAddSpecificUsage(Constant.BusinessPackageItem.Member, memberSearch.CompanyId, -1, addAfterSubscriptionValid: true);
                if (!subscriptionStatus.Equals(Constant.BusinessUsageStatus.OK))
                    return subscriptionStatus.ToString();

                //rejectmember
                memberSearch.IsActive = false;
                memberSearch.IsCompanyAccept = false;
                db.SaveChanges();
                return subscriptionStatus.ToString();

            }
        }


        public long Save(Company company)
        {

            using (var db = new ServiceContext())
            {

                while (checkIdExist(company.Id))
                {
                    company.Id = Utilities.RandomLongGenerator(minimumValue: Constant.MINIMUM_VALUE_ID, maximumValue: Constant.MAXIMUM_VALUE_ID);
                }
                db.Companies.Add(company);
                db.SaveChanges();

                return company.Id;
            }
        }

        private bool checkIdExist(long id)
        {
            using (var db = new ServiceContext())
            {

                var count = db.Companies.Where(i => i.Id == id).FirstOrDefault();

                return count != null;

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

    }
}