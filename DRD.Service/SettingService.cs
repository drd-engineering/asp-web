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
using DRD.Models.View;

namespace DRD.Service
{
    public class SettingService
    {
        SubscriptionService subscriptionService = new SubscriptionService();
        public CompanySettingData getCompanySetting(long userId)
        {
            using (var db = new ServiceContext())
            {
                CompanySettingData companyData = new CompanySettingData();
                var pendingData = (from member in db.Members
                            orderby member.isMemberAccept
                            join company in db.Companies on  member.CompanyId equals company.Id
                            join user in db.Users on company.OwnerId equals user.Id
                            where member.UserId == userId && member.isCompanyAccept && member.IsActive && company.OwnerId != member.UserId
                                   select new CompanyItemMember
                            {
                                Id = company.Id,
                                Name = company.Name,
                                OwnerId = company.OwnerId,
                                OwnerName = user.Name,
                                       Role = member.isMemberAccept ? (member.IsAdministrator ? Constant.MemberRole.Administrator.ToString() : Constant.MemberRole.Member.ToString()) : Constant.MemberRole.Not_Member.ToString(),
                                Status = member.isMemberAccept ? (int)Constant.InivitationStatus.Connected : (int)Constant.InivitationStatus.Pending
                            }).ToList();
                if (pendingData != null)
                {
                    companyData.companies = pendingData;
                }
                return companyData;
            }
        }

        public int acceptCompany(long companyId, long userId)
        {
            using (var db = new ServiceContext())
            {
                var member = db.Members.Where(i => i.UserId == userId && i.CompanyId == companyId).FirstOrDefault();

                if (member == null) return (int)Constant.InivitationStatus.ERROR_NOT_FOUND;

                var subscriptionStatus = subscriptionService.CheckOrAddSpecificUsage(Constant.BusinessPackageItem.Member, companyId, 1, addAfterSubscriptionValid: true);
                if (!subscriptionStatus.Equals(Constant.BusinessUsageStatus.OK)) return (int)subscriptionStatus;


                member.isMemberAccept = true;
                db.SaveChanges();
                return (int)subscriptionStatus;
            }
        }

        public string resetState(long companyId, long userId)
        {
            using (var db = new ServiceContext())
            {
                var member = db.Members.Where(i => i.UserId == userId && i.CompanyId == companyId).FirstOrDefault();
                if (member == null) return Constant.InivitationStatus.ERROR_NOT_FOUND.ToString();

                var subscriptionStatus = subscriptionService.CheckOrAddSpecificUsage(Constant.BusinessPackageItem.Member, companyId, -1, addAfterSubscriptionValid: true);
                if (!subscriptionStatus.Equals(Constant.BusinessUsageStatus.OK)) return subscriptionStatus.ToString();

                member.IsActive = false;
                //reset state
                member.isCompanyAccept = false;
                member.IsAdministrator = false;
                member.isMemberAccept = false;
                db.SaveChanges();
                return subscriptionStatus.ToString();
            }
        }
    }
}
