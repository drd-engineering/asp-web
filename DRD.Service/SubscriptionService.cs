using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using DRD.Models;
using DRD.Models.API;
using DRD.Service.Context;

namespace DRD.Service
{
    public class SubscriptionService
    {
        public List<BusinessSubscription> getSubscription()
        {
            var root = System.Web.HttpContext.Current.Server.MapPath("~");
            var path = Path.Combine(root, @"Subscription.csv");
            List<BusinessSubscription> values = File.ReadAllLines(path)
                                           .Select(v => BusinessSubscription.FromCsv(v))
                                           .ToList();
            return values;
        }

        public string getSubscriptionName(long subscriptionId)
        {
            List<BusinessSubscription> businessSubscriptions = getSubscription();
            return businessSubscriptions[(int)subscriptionId - 1].Name;
        }

        public bool deactivatePlanBusiness(long companyId)
        {
            PlanBusiness planBusiness = editPlanBusiness(companyId,null,null,null, IsActive: false);
            return planBusiness != null;
        }

        public PlanBusiness editPlanBusiness(long companyId, int? TotalAdministrators, DateTime? ExpiredAt, long? Price, bool? IsActive)
        {
            using (var db = new ServiceContext())
            {
                PlanBusiness planBusiness = (from c in db.PlanBusinesses
                 where c.CompanyId == companyId && c.IsActive && c.StorageUsedinByte > 0
                 select c).FirstOrDefault();
                planBusiness.totalAdministrators = (TotalAdministrators.HasValue ? TotalAdministrators.Value : planBusiness.totalAdministrators);
                planBusiness.ExpiredAt = (ExpiredAt.HasValue ? ExpiredAt.Value : planBusiness.ExpiredAt);
                planBusiness.Price = (Price.HasValue ? Price.Value : planBusiness.Price);
                planBusiness.IsActive = (IsActive.HasValue ? IsActive.Value : planBusiness.IsActive);
                db.SaveChanges();
                return planBusiness;
            }
        }

        public bool isSubscriptionValid(long userId, long subscriptionId)
        {
            using (var db = new ServiceContext())
            {
                var plan =
                    (from plandb in db.PlanBusinesses
                     where plandb.Id == subscriptionId && plandb.IsActive && plandb.StorageUsedinByte > 0
                     select new
                     {
                         Id = plandb.Id,
                         IsActive = plandb.IsActive,
                         ExpiredAt = plandb.ExpiredAt,
                         companyId = plandb.CompanyId
                     }).FirstOrDefault();
                if(plan != null && plan.ExpiredAt < DateTime.Now)
                {
                    deactivatePlanBusiness(subscriptionId);
                    return false;
                }
                var member =
                    (from memberdb in db.Members
                     join company in db.Companies on memberdb.CompanyId equals company.Id
                     where memberdb.UserId == userId
                     select new
                     {
                         Id = memberdb.Id
                     }).FirstOrDefault();
                return (plan != null)&&(member != null);
            }
        }

        public BusinessSubscriptionList getBusinessSubscriptionByUser(long userId)
        {
            using (var db = new ServiceContext())
            {
                var returnList = new BusinessSubscriptionList();
                var OwnerSubscriptions = new BusinessSubscriptionList();


                var adminSubscription = (from member in db.Members
                                            join company in db.Companies on member.CompanyId equals company.Id
                                            join plan in db.PlanBusinesses on company.Id equals plan.CompanyId
                                            where member.UserId == userId && plan.IsActive && member.IsAdministrator
                                            select new BusinessSubscriptionItem
                                            { 
                                                Id = plan == null ? 0 : plan.Id,
                                                CompanyId = company == null ? 0 : company.Id,
                                                CompanyName = company == null ? null : company.Name,
                                                StorageUsedinByte = plan == null ? 0 : plan.StorageUsedinByte,
                                                SubscriptionName = plan == null ? null : plan.SubscriptionName,
                                                totalAdministrators = plan == null ? 0 : plan.totalAdministrators
                                            }).ToList();
                returnList.subscriptions = adminSubscription;
                System.Diagnostics.Debug.WriteLine("COUNT :: ADMIN :: " + returnList.subscriptions.Count);
                var ownerSubscriptions = (from  company in db.Companies
                                          join plan in db.PlanBusinesses on company.Id equals plan.CompanyId
                                          where company.OwnerId == userId && plan.IsActive
                                         select new BusinessSubscriptionItem
                                         {
                                             Id = plan == null ? 0 : plan.Id,
                                             CompanyId = company == null ? 0 : company.Id,
                                             CompanyName = company == null ? null : company.Name,
                                             StorageUsedinByte = plan == null ? 0 : plan.StorageUsedinByte,
                                             SubscriptionName = plan == null ? null : plan.SubscriptionName,
                                             totalAdministrators = plan == null ? 0 : plan.totalAdministrators
                                         }).ToList();
                OwnerSubscriptions.subscriptions = ownerSubscriptions;
                System.Diagnostics.Debug.WriteLine("COUNT :: TOTAL :: " + returnList.subscriptions.Count);
                returnList.mergeBusinessSubscriptionList(OwnerSubscriptions);
                return returnList;
            }
        }
    }
}
