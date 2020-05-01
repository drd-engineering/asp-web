using DRD.Models;
using DRD.Models.API;
using DRD.Models.View;
using DRD.Service.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DRD.Service
{
    public class SubscriptionService
    {
        public bool DeactivateActiveUsage(long companyId)
        {
            Usage planBusiness = EditBusinessPackage(companyId, null, null, null, IsActive: false);
            return planBusiness != null;
        }

        public Usage EditBusinessPackage(long companyId, int? TotalAdministrators, DateTime? ExpiredAt, long? Price, bool? IsActive)
        {
            using (var db = new ServiceContext())
            {
                Usage planBusiness = (from c in db.Usages
                                      where c.CompanyId == companyId && c.IsActive
                                      select c).FirstOrDefault();
                planBusiness.Administrator = (TotalAdministrators.HasValue ? TotalAdministrators.Value : planBusiness.Administrator);
                planBusiness.ExpiredAt = (ExpiredAt.HasValue ? ExpiredAt.Value : planBusiness.ExpiredAt);
                planBusiness.IsActive = (IsActive.HasValue ? IsActive.Value : planBusiness.IsActive);
                db.SaveChanges();
                return planBusiness;
            }
        }

        public ActiveUsage GetActiveBusinessSubscriptionByCompany(long companyId)
        {
            using (var db = new ServiceContext())
            {
                return (from company in db.Companies
                        join usage in db.Usages on company.Id equals usage.CompanyId
                        join price in db.Prices on usage.PriceId equals price.Id
                        join package in db.BusinessPackages on usage.PackageId equals package.Id
                        where company.Id == companyId && usage.IsActive
                        select new ActiveUsage
                        {
                            Id = usage == null ? 0 : usage.Id,
                            AdministratorsLimit = package == null ? 0 : package.Administrator,
                            CompanyId = company == null ? 0 : company.Id,
                            CompanyName = company == null ? null : company.Name,
                            ExpiredAt = usage == null ? DateTime.MinValue : usage.ExpiredAt,
                            IsActive = usage == null ? false : usage.IsActive,
                            PackageName = package == null ? null : package.Name,
                            RotationLimit = package == null ? 0 : package.Rotation,
                            RotationStartedLimit = package == null ? 0 : package.RotationStarted,
                            StartedAt = usage == null ? DateTime.MinValue : usage.StartedAt,
                            StorageLimit = package == null ? 0 : package.Storage,
                            TotalAdministrators = usage == null ? 0 : usage.Administrator,
                            TotalPrice = price == null ? 0 : price.Total,
                            TotalRotation = usage == null ? 0 : usage.Rotation,
                            TotalRotationStarted = usage == null ? 0 : usage.RotationStarted,
                            TotalStorage = usage == null ? 0 : usage.Storage,
                            TotalUsers = usage == null ? 0 : usage.User,
                            TotalWorkflow = usage == null ? 0 : usage.Workflow,
                            UsersLimit = package == null ? 0 : package.User,
                            WorkflowLimit = package == null ? 0 : package.Workflow,
                        }).FirstOrDefault();
            }
        }

        public List<BusinessPackage> GetAllPublicSubscription()
        {
            using (var db = new ServiceContext())
            {
                return db.BusinessPackages.Where(bs => bs.IsPublic && bs.IsActive).ToList();
            }
        }

        public ActiveUsageList GetBusinessSubscriptionByUser(long userId)
        {
            using (var db = new ServiceContext())
            {
                var returnList = new ActiveUsageList();
                var OwnerSubscriptions = new ActiveUsageList();

                var adminSubscription = (from member in db.Members
                                         join company in db.Companies on member.CompanyId equals company.Id
                                         join usage in db.Usages on company.Id equals usage.CompanyId
                                         join package in db.BusinessPackages on usage.PackageId equals package.Id
                                         where member.UserId == userId && usage.IsActive && member.IsAdministrator
                                         select new ActiveUsage
                                         {
                                             Id = usage == null ? 0 : usage.Id,
                                             CompanyId = company == null ? 0 : company.Id,
                                             CompanyName = company == null ? null : company.Name,
                                             StorageLimit = package == null ? 0 : package.Storage,
                                             PackageName = package == null ? null : package.Name,
                                             TotalAdministrators = usage == null ? 0 : usage.Administrator
                                         }).ToList();
                returnList.usages = adminSubscription;
                System.Diagnostics.Debug.WriteLine("COUNT :: ADMIN :: " + returnList.usages.Count);
                var ownerSubscriptions = (from company in db.Companies
                                          join usage in db.Usages on company.Id equals usage.CompanyId
                                          join package in db.BusinessPackages on usage.PackageId equals package.Id
                                          where company.OwnerId == userId && usage.IsActive
                                          select new ActiveUsage
                                          {
                                              Id = usage == null ? 0 : usage.Id,
                                              CompanyId = company == null ? 0 : company.Id,
                                              CompanyName = company == null ? null : company.Name,
                                              StorageLimit = package == null ? 0 : package.Storage,
                                              PackageName = package == null ? null : package.Name,
                                              TotalAdministrators = usage == null ? 0 : usage.Administrator
                                          }).ToList();
                OwnerSubscriptions.usages = ownerSubscriptions;
                System.Diagnostics.Debug.WriteLine("COUNT :: TOTAL :: " + returnList.usages.Count);
                returnList.mergeBusinessSubscriptionList(OwnerSubscriptions);
                returnList.usages = returnList.usages.OrderBy(i => i.CompanyName).ToList();
                return returnList;
            }
        }

        public BusinessPackage GetCompanyPackage(long packageId)
        {
            using (var db = new ServiceContext())
            {
                BusinessPackage plan = db.BusinessPackages.Where(c => c.Id == packageId).FirstOrDefault();
                return plan;
            }
        }

        public BusinessPackage getCompanyPackageByCompany(long companyId)
        {
            Usage usage = GetCompanyUsage(companyId);
            return GetCompanyPackage(usage.PackageId);
        }

        public Usage GetCompanyUsage(long companyId)
        {
            using (var db = new ServiceContext())
            {
                Usage plan = db.Usages.Where(c => c.CompanyId == companyId && c.IsActive).FirstOrDefault();
                return plan;
            }
        }

        public Usage getCompanyUsageById(long id)
        {
            using (var db = new ServiceContext())
            {
                Usage plan = db.Usages.Where(c => c.Id == id).FirstOrDefault();
                return plan;
            }
        }

        public bool IsSubscriptionValid(long userId, long usageId)
        {
            using (var db = new ServiceContext())
            {
                Usage usage = getCompanyUsageById(usageId);

                //check expiration date
                if (usage != null)
                {
                    var package = GetCompanyPackage(usage.PackageId);
                    if (!IsValid(package, usage))
                    {
                        DeactivateActiveUsage(usageId);
                        return false;
                    }
                }
                var member =
                    (from memberdb in db.Members
                     join company in db.Companies on memberdb.CompanyId equals company.Id
                     where memberdb.UserId == userId
                     select new
                     {
                         Id = memberdb.Id
                     }).FirstOrDefault();
                return (usage != null) && (member != null);
            }
        }

        public bool IsValid(BusinessPackage package, Usage usage)
        {
            if (!package.IsExceedLimitAllowed)
            {
                if (package.Administrator != -99 && usage.Administrator > package.Administrator) return false;
                if (package.Rotation != -99 && usage.Rotation > package.Rotation) return false;
                if (package.RotationStarted != -99 && usage.RotationStarted > package.RotationStarted) return false;
                if (package.Storage != -99 && usage.Storage > package.Storage) return false;
                if (package.User != -99 && usage.User > package.User) return false;
            }
            if (!package.IsExpirationDateExtendedAutomatically)
            {
                if (usage.ExpiredAt < DateTime.Now) return false;
            }
            return true;
        }
    }
}