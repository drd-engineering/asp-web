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
using DRD.Models.API.Register;
using DRD.Service;
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
                var planBusiness = (from c in db.PlanBusinesses
                 where c.CompanyId == companyId && c.IsActive && c.StorageUsedinByte > 0
                 select new PlanBusiness
                 {
                     Id = c.Id,
                     totalAdministrators = TotalAdministrators == null? c.totalAdministrators : (int)TotalAdministrators,
                     CompanyId = c.CompanyId,
                     ExpiredAt = ExpiredAt == null? c.ExpiredAt : ExpiredAt,
                     Price = Price==null? c.Price : (long)Price,
                     StartedAt= c.StartedAt,
                     StorageUsedinByte = c.StorageUsedinByte,
                     SubscriptionName = c.SubscriptionName,
                     IsActive = IsActive==null? c.IsActive: (bool)IsActive,
                 }).FirstOrDefault();
                db.PlanBusinesses.Add(planBusiness);
                db.SaveChanges();
                return planBusiness;
            }
        }

        public bool isSubscriptionValid(long subscriptionId, bool isCompanySubscription)
        {
            using (var db = new ServiceContext())
            {
                if (!isCompanySubscription)
                {
                    var user =
                    (from c in db.Users
                     where c.Id == subscriptionId
                     select new User
                     {
                         Id = c.Id,
                         Name = c.Name,
                         Phone = c.Phone,
                         Email = c.Email,
                         ImageProfile = c.ImageProfile,
                         IsActive = c.IsActive,
                         
                     }).FirstOrDefault();
                    return true;
                    
                }

                var company =
                    (from c in db.PlanBusinesses
                     where c.CompanyId == subscriptionId && c.IsActive && c.StorageUsedinByte > 0
                     select new PlanBusiness
                     {
                         Id = c.Id,
                         IsActive = c.IsActive,
                     }).FirstOrDefault();
                if(company != null && company.ExpiredAt >= DateTime.Now)
                {
                    deactivatePlanBusiness(subscriptionId);
                    return false;
                }
                    return company != null;
                
            }
        }

        public List<BusinessSubscriptionItem> getBusinessSubscriptionByUser(long userId)
        {
            using (var db = new ServiceContext())
            {
                var returnList = new List<BusinessSubscriptionItem>();
                var OwnerSubscriptions = new BusinessSubscriptionList();
                                            //{returnList.subscriptions = (from member in db.Members
                                            //join company in db.Companies on member.CompanyId equals company.Id
                                            //join plan in db.PlanBusinesses on company.Id equals plan.CompanyId
                                            //where member.UserId == userId && plan.IsActive
                                            //select new BusinessSubscriptionItem
                                            //{

                returnList = (from member in db.Members
                                            join company in db.Companies on member.CompanyId equals company.Id
                                            //join plan in db.PlanBusinesses on company.Id equals plan.CompanyId
                                            //where member.UserId == userId && plan.IsActive
                                            select new BusinessSubscriptionItem
                                            { 
                                                //Id = plan == null ? 0 : plan.Id,
                                                CompanyId = company == null ? 0 : company.Id,
                                                CompanyName = company == null ? null : company.Name,
                                                //StorageUsedinByte = plan == null ? 0 : plan.StorageUsedinByte,
                                                //SubscriptionName = plan == null ? null : plan.SubscriptionName,
                                                //totalAdministrators = plan == null ? 0 : plan.totalAdministrators
                                            }).ToList();
                return returnList;
                //System.Diagnostics.Debug.WriteLine("COUNT :: owner" + returnList.subscriptions.Count);

                //var ownerSubscriptions = (from plan in db.PlanBusinesses
                //                         join company in db.Companies on plan.CompanyId equals company.Id
                //                         where company.OwnerId == userId && plan.IsActive
                //                         select new BusinessSubscriptionItem
                //                         {
                //                             Id = plan.Id,
                //                             CompanyId = company.Id,
                //                             CompanyName = company.Name,
                //                             StorageUsedinByte = plan.StorageUsedinByte,
                //                             SubscriptionName = plan.SubscriptionName,
                //                             totalAdministrators = plan.totalAdministrators
                //                         }).ToList();
                //OwnerSubscriptions.subscriptions = ownerSubscriptions;
                //System.Diagnostics.Debug.WriteLine("COUNT :: TOTAL :: " + returnList.subscriptions.Count);
                //returnList.mergeBusinessSubscriptionList(OwnerSubscriptions);
                //return returnList;
            }
        }
    }
}
