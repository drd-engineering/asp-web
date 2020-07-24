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
        private SubscriptionService subscriptionService;
        private UserService userService;

        //helper
        private ICollection<CompanyItem> GetAllCompanyDetailByAdminId(long adminId)
        {
            memberService = new MemberService();
            using var db = new Connection();
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

        //helper
        private ICollection<CompanyItem> GetAllCompanyDetailByOwnerId(long ownerId)
        {
            using var db = new Connection();
            memberService = new MemberService();
            userService = new UserService();
            subscriptionService = new SubscriptionService();

            var ownerCompanies = db.Companies.Where(companyItem => companyItem.OwnerId == ownerId && companyItem.IsActive).ToList();
            var listReturn = new List<CompanyItem>();
            foreach (Company x in ownerCompanies)
            {
                var subscription = subscriptionService.GetCompanyUsage(x.Id);
                var company = new CompanyItem
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Phone = x.Phone,
                    Address = x.Address,
                    PointLocation = x.MapCoordinate,
                    OwnerId = x.OwnerId,
                    IsActive = x.IsActive,
                    IsVerified = x.IsVerified,
                    IsOwnedByUser = true
                };
                company.OwnerName = userService.GetName(company.OwnerId);
                company.Administrators = memberService.GetAdministrators(company.Id);
                if (subscription != null)
                {
                    company.SubscriptionId = subscription.Id;
                    BusinessUsage usage = db.BusinessUsages.Where(y => y.Id == subscription.Id && y.IsActive).FirstOrDefault();
                    company.SubscriptionName = db.BusinessPackages.Where(package => package.Id == usage.PackageId).Select(i => i.Name).FirstOrDefault();
                }
                listReturn.Add(company);
            }
            return listReturn;
        }

        /// <summary>
        /// Accept user to be a member of the company
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public string AcceptMember(long memberId, long loginUserId)
        {
            using var db = new Connection();
            subscriptionService = new SubscriptionService();

            Member memberSearch = db.Members.Where(memberItem => memberItem.Id == memberId).FirstOrDefault();
            
            if (memberSearch == null) return Constant.InivitationStatus.ERROR_NOT_FOUND.ToString();
            
            memberSearch.IsCompanyAccept = true;
            var subscriptionStatus = subscriptionService.CheckOrAddSpecificUsage(Models.Constant.BusinessPackageItem.Member, memberSearch.CompanyId, 1, addAfterSubscriptionValid: true);

            if (subscriptionStatus.Equals(Constant.BusinessUsageStatus.OK))
            {
                AuditTrailService.RecordLog(loginUserId, Constant.AuditTrail.Company.ToString(), AuditTrailMessages.AcceptMember(memberSearch.UserId, memberSearch.CompanyId));
                db.SaveChanges();
            }
            
            return subscriptionStatus.ToString();    
        }

        /// <summary>
        /// GET all company available in DRD 
        /// </summary>
        /// <returns>only contains little details like id, name and code</returns>
        public ICollection<SmallCompanyData> GetCompanies()
        {
            using var db = new Connection();
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

        /// <summary>
        /// GET all company that user manage and own
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ICollection<CompanyItem> GetOwnedandManagedCompany(long userId)
        {
            using var db = new Connection();
            var managedCompany = GetAllCompanyDetailByAdminId(userId);
            var ownedCompany = GetAllCompanyDetailByOwnerId(userId);
            // merge two list of company
            List<CompanyItem> listReturn = new List<CompanyItem>();
            listReturn.AddRange(ownedCompany);
            listReturn.AddRange(managedCompany);
            return listReturn;
        }
        
     
        /// <summary>
        /// GET all company that user has
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>only contain small data like id, code and name of company</returns>
        public ICollection<SmallCompanyData> GetCompaniesOwnedByUser(long userId)
        {
            using var db = new Connection();
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

        //helper
        private CompanyItem GetCompanyDetail(long id)
        {
            using var db = new Connection();
            memberService = new MemberService();
            userService = new UserService();
            subscriptionService = new SubscriptionService();


            var result = db.Companies.Where(companyItem => companyItem.Id == id).FirstOrDefault();
            var subscription = subscriptionService.GetCompanyUsage(result.Id);
            if (result == null) return null;
            var company = new CompanyItem
            {
                Id = result.Id,
                Code = result.Code,
                Name = result.Name,
                Phone = result.Phone,
                Address = result.Address,
                PointLocation = result.MapCoordinate,
                OwnerId = result.OwnerId,
                IsActive = result.IsActive,
                IsVerified = result.IsVerified
            };
            company.OwnerName = userService.GetName(company.OwnerId);
            company.Administrators = memberService.GetAdministrators(company.Id);
            if (subscription != null)
            {
                company.SubscriptionId = subscription.Id;
                BusinessUsage usage = db.BusinessUsages.Where(y => y.Id == subscription.Id && y.IsActive).FirstOrDefault();
                company.SubscriptionName = db.BusinessPackages.Where(package => package.Id == usage.PackageId).Select(i => i.Name).FirstOrDefault();
            }
            return company;
        }
        /// <summary>
        /// GET company little details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SmallCompanyData GetCompany(long id, long userId = Constant.ID_NOT_FOUND)
        {

            memberService = new MemberService();
            if (userId != Constant.ID_NOT_FOUND && !memberService.CheckIsAdmin(userId, id) && !memberService.CheckIsOwner(userId, id))
                return null;

            using var db = new Connection();
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
        
        /// <summary>
        /// Reject a member request to join the company
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns> return user id if member rejected, return -1 if member not found. </returns>
        public string RejectMember(long memberId, long loginUserId)
        {
            using var db = new Connection();
            subscriptionService = new SubscriptionService();

            Member memberSearch = db.Members.Where(memberItem => memberItem.Id == memberId).FirstOrDefault();
            //error not found
            if (memberSearch == null) return Constant.InivitationStatus.ERROR_NOT_FOUND.ToString();

            //check subscription
            var subscriptionStatus = subscriptionService.CheckOrAddSpecificUsage(Models.Constant.BusinessPackageItem.Member, memberSearch.CompanyId, -1, addAfterSubscriptionValid: true);
            if (!subscriptionStatus.Equals(Constant.BusinessUsageStatus.OK))
                return subscriptionStatus.ToString();

            //rejectmember
            memberSearch.IsActive = false;
            memberSearch.IsCompanyAccept = false;

            AuditTrailService.RecordLog(loginUserId, Constant.AuditTrail.Company.ToString(), AuditTrailMessages.AcceptMember(memberSearch.UserId, memberSearch.CompanyId));

            db.SaveChanges();
            return subscriptionStatus.ToString();
        }

        /// <summary>
        /// Add members invitation and send email
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="userId"></param>
        /// <param name="emails"></param>
        /// <returns></returns>
        public List<AddMemberResponse> AddMembers(long companyId, long userId, string emails, long loginUserId)
        {

            memberService = new MemberService();
            var data = memberService.AddMembers(companyId, userId, emails, loginUserId);
            foreach (AddMemberResponse item in data)
                memberService.SendEmailAddMember(item);
            
            return data;
        }
    }
}