using DRD.Models;
using DRD.Models.API;
using DRD.Service.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DRD.Service
{
    public class SubscriptionService
    {
        UserService userService = new UserService();

        public Constant.BusinessUsageStatus CheckOrAddSpecificUsage(Constant.BusinessPackageItem packageType,  long companyId, long? additional = 0, bool? addAfterSubscriptionValid = false, bool reset = false)
        {
            using (var db = new ServiceContext())
            {

                BusinessUsage usage = db.BusinessUsages.Where(c => c.CompanyId == companyId && c.IsActive).FirstOrDefault();
                BusinessPackage package = db.BusinessPackages.Where(c => c.Id == usage.PackageId && c.IsActive).FirstOrDefault();
                int additionalUsage = 0;

                if (!packageType.Equals(Constant.BusinessPackageItem.Storage))
                {
                    additionalUsage = (int)additional;
                }

                if (!checkExpired(package, usage).Equals(Constant.BusinessUsageStatus.OK))
                {
                    return Constant.BusinessUsageStatus.EXPIRED;
                }

                switch (packageType)
                {
                    case Constant.BusinessPackageItem.Administrator:
                        if (!package.IsExceedLimitAllowed && package.Administrator != Constant.ALLOW_EXCEED_LIMIT && ((!reset && usage.Administrator + additionalUsage > package.Administrator) || (reset && additionalUsage > package.Administrator)))
                            return Constant.BusinessUsageStatus.ADMINISTRATOR_EXCEED_LIMIT;
                        else if (addAfterSubscriptionValid.Value)
                            usage.Administrator = reset ? additionalUsage : additionalUsage + usage.Administrator;
                        break;
                    case Constant.BusinessPackageItem.Rotation_Started:
                        if (!package.IsExceedLimitAllowed && package.RotationStarted != Constant.ALLOW_EXCEED_LIMIT && ((!reset && usage.RotationStarted + additionalUsage > package.RotationStarted) || (reset && additionalUsage > package.RotationStarted)))
                            return Constant.BusinessUsageStatus.ROTATION_STARTED_EXCEED_LIMIT;
                        else if (addAfterSubscriptionValid.Value)
                            usage.RotationStarted = reset? additionalUsage: additionalUsage + usage.RotationStarted;
                        break;
                    case Constant.BusinessPackageItem.Member:
                        if (!package.IsExceedLimitAllowed && package.Member != Constant.ALLOW_EXCEED_LIMIT && ((!reset && usage.Member + additionalUsage > package.Member) || (reset && additionalUsage > package.Member)))
                            return Constant.BusinessUsageStatus.MEMBER_EXCEED_LIMIT;
                        else if (addAfterSubscriptionValid.Value)
                            usage.Member = reset ? additionalUsage : additionalUsage + usage.Member;
                        break;
                    case Constant.BusinessPackageItem.Storage:
                        if (!package.IsExceedLimitAllowed && package.Storage != Constant.ALLOW_EXCEED_LIMIT && ((!reset && usage.Storage + additionalUsage > package.Storage) || (reset && additionalUsage > package.Storage)))
                            return Constant.BusinessUsageStatus.STORAGE_EXCEED_LIMIT;
                        else if (addAfterSubscriptionValid.Value)
                            usage.Storage = reset ? additionalUsage : additionalUsage + usage.Storage;
                        break;
                }
              
                db.SaveChanges();
            }
            return Constant.BusinessUsageStatus.OK;
        }

        public BusinessUsage DeactivateActiveUsage(long companyId)
        {
            BusinessUsage planBusiness = EditBusinessPackage(companyId, null, null, null, IsActive: false);
            return planBusiness;
        }

        public BusinessUsage EditBusinessPackage(long companyId, int? TotalAdministrators, DateTime? ExpiredAt, long? Price, bool? IsActive)
        {
            using (var db = new ServiceContext())
            {
                BusinessUsage planBusiness = (from c in db.BusinessUsages
                                      where c.CompanyId == companyId && c.IsActive
                                      select c).FirstOrDefault();
                if (planBusiness != null)
                {
                    planBusiness.Administrator = (TotalAdministrators.HasValue ? TotalAdministrators.Value : planBusiness.Administrator);
                    planBusiness.ExpiredAt = (ExpiredAt.HasValue ? ExpiredAt.Value : planBusiness.ExpiredAt);
                    planBusiness.IsActive = (IsActive.HasValue ? IsActive.Value : planBusiness.IsActive);
                    db.SaveChanges();
                }
                return planBusiness;
            }
        }

        public ActiveUsage GetActiveBusinessSubscriptionByCompany(long companyId)
        {
            using (var db = new ServiceContext())
            {
                return (from company in db.Companies
                        join usage in db.BusinessUsages on company.Id equals usage.CompanyId
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
                            RotationStartedLimit = package == null ? 0 : package.RotationStarted,
                            StartedAt = usage == null ? DateTime.MinValue : usage.CreatedAt,
                            StorageLimit = package == null ? 0 : package.Storage,
                            TotalAdministrators = usage == null ? 0 : usage.Administrator,
                            TotalPrice = price == null ? 0 : price.Total,
                            TotalRotationStarted = usage == null ? 0 : usage.RotationStarted,
                            TotalStorage = usage == null ? 0 : usage.Storage,
                            TotalUsers = usage == null ? 0 : usage.Member,
                            UsersLimit = package == null ? 0 : package.Member,
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
                                         join usage in db.BusinessUsages on company.Id equals usage.CompanyId
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
                var ownerSubscriptions = (from company in db.Companies
                                          join usage in db.BusinessUsages on company.Id equals usage.CompanyId
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
                returnList.mergeBusinessSubscriptionList(OwnerSubscriptions);
                returnList.usages = returnList.usages.OrderBy(i => i.CompanyName).ToList();
                return returnList;
            }
        }

        internal Price getActivePricePackage(long packageId)
        {
            using (var db = new ServiceContext())
            {
                return db.Prices.Where(p => p.PackageId == packageId && p.IsActive).LastOrDefault();
            }
        }

        public BusinessPackage GetCompanyPackage(long packageId)
        {
            using (var db = new ServiceContext())
            {
                BusinessPackage plan = db.BusinessPackages.Where(c => c.Id == packageId && c.IsActive).FirstOrDefault();
                return plan;
            }
        }

        public BusinessPackage getCompanyPackageByCompany(long companyId)
        {
            BusinessUsage usage = GetCompanyUsage(companyId);
            return GetCompanyPackage(usage.PackageId);
        }

        public BusinessUsage GetCompanyUsage(long companyId)
        {
            using (var db = new ServiceContext())
            {
                BusinessUsage plan = db.BusinessUsages.Where(c => c.CompanyId == companyId && c.IsActive).FirstOrDefault();
                return plan;
            }
        }

        public BusinessUsage getCompanyUsageById(long id)
        {
            using (var db = new ServiceContext())
            {
                BusinessUsage plan = db.BusinessUsages.Where(c => c.Id == id).FirstOrDefault();
                return plan;
            }
        }

        public Constant.BusinessUsageStatus IsSubscriptionValid(long userId, long usageId)
        {
            using (var db = new ServiceContext())
            {
                BusinessUsage usage = getCompanyUsageById(usageId);
                //check if having any active usage
                if (usage == null) return Constant.BusinessUsageStatus.NO_ACTIVE_PLAN;
                if (usage != null)
                {
                    var package = GetCompanyPackage(usage.PackageId);
                    //check usage with package limitation
                    Constant.BusinessUsageStatus validStatus = IsValid(package, usage);
                    if ( validStatus != Constant.BusinessUsageStatus.OK)
                    {
                        return validStatus;
                    }
                }
                //check if authorizeed admin or owner
                if (!userService.IsAdminOrOwnerofSpecificCompany(userId,companyId:usage.CompanyId)) return Constant.BusinessUsageStatus.NOT_AUTHORIZED;
                return Constant.BusinessUsageStatus.OK;
            }
        }

        public Constant.BusinessUsageStatus IsValid(BusinessPackage package, BusinessUsage usage)
        {
            if (!package.IsExceedLimitAllowed)
            {
                if (package.Administrator != Constant.ALLOW_EXCEED_LIMIT && usage.Administrator > package.Administrator) return Constant.BusinessUsageStatus.ADMINISTRATOR_EXCEED_LIMIT;
                if (package.RotationStarted != Constant.ALLOW_EXCEED_LIMIT && usage.RotationStarted > package.RotationStarted) return Constant.BusinessUsageStatus.ROTATION_STARTED_EXCEED_LIMIT;
                if (package.Storage != Constant.ALLOW_EXCEED_LIMIT && usage.Storage > package.Storage) return Constant.BusinessUsageStatus.STORAGE_EXCEED_LIMIT;
                if (package.Member != Constant.ALLOW_EXCEED_LIMIT && usage.Member > package.Member) return Constant.BusinessUsageStatus.MEMBER_EXCEED_LIMIT;
            }


            return checkExpired(package,usage);
        }

        public Constant.BusinessUsageStatus checkExpired(BusinessPackage package, BusinessUsage usage)
        {
            if (!package.IsExpirationDateExtendedAutomatically && usage.ExpiredAt < DateTime.Now)
            {
                DeactivateActiveUsage(usage.CompanyId);
                return Constant.BusinessUsageStatus.EXPIRED;
            }
            if (package.IsExpirationDateExtendedAutomatically && usage.ExpiredAt < DateTime.Now)
            {
                ExtendUsage(usage.Id);
            }
            return Constant.BusinessUsageStatus.OK;
        }

        public void ExtendUsage(long companyId)
        {
            BusinessUsage oldUsage = DeactivateActiveUsage(companyId);
            BusinessPackage businessPackage = getCompanyPackageByCompany(companyId);
            using (var db = new ServiceContext())
            {
                DateTime? extendedTime = oldUsage.ExpiredAt.Value.AddDays(businessPackage.Duration);
                BusinessUsage newUsage = new BusinessUsage(oldUsage, startedAt: oldUsage.ExpiredAt, expiredAt: extendedTime);

                db.BusinessUsages.Add(newUsage);
                db.SaveChanges();
            }
        }
    }
}