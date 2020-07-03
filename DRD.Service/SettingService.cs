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
        SubscriptionService subscriptionService;

        /// <summary>
        /// get setting for company
        /// </summary>
        /// <param name="companyId"></param
        /// <param name="userId"></param
        /// <returns></returns>
        public CompanySettingData GetCompanySetting(long userId)
        {
            using var db = new ServiceContext();
            CompanySettingData companyData = new CompanySettingData();
            var pendingData = (from member in db.Members
                               orderby member.IsMemberAccept
                               join company in db.Companies on member.CompanyId equals company.Id
                               join user in db.Users on company.OwnerId equals user.Id
                               where member.UserId == userId && member.IsCompanyAccept && member.IsActive && company.OwnerId != member.UserId
                               select new CompanyItemMember
                               {
                                   Id = company.Id,
                                   Name = company.Name,
                                   OwnerId = company.OwnerId,
                                   OwnerName = user.Name,
                                   Role = member.IsMemberAccept ? (member.IsAdministrator ? ConstantModel.MemberRole.Administrator.ToString() : ConstantModel.MemberRole.Member.ToString()) : ConstantModel.MemberRole.Not_Member.ToString(),
                                   Status = member.IsMemberAccept ? (int)Constant.InivitationStatus.Connected : (int)Constant.InivitationStatus.Pending
                               }).ToList();
            if (pendingData != null)
                companyData.companies = pendingData;
            return companyData;
        }
        /// <summary>
        /// member accept company invitation
        /// </summary>
        /// <param name="companyId"></param
        /// <param name="userId"></param
        /// <returns></returns>
        public string AcceptCompany(long companyId, long userId)
        {
            using var db = new ServiceContext();
            var member = db.Members.Where(i => i.UserId == userId && i.CompanyId == companyId).FirstOrDefault();

            if (member == null) return Constant.InivitationStatus.ERROR_NOT_FOUND.ToString();

            var subscriptionStatus = subscriptionService.CheckOrAddSpecificUsage(ConstantModel.BusinessPackageItem.Member, companyId, 1, addAfterSubscriptionValid: true);
            if (!subscriptionStatus.Equals(Constant.BusinessUsageStatus.OK)) return subscriptionStatus.ToString();


            member.IsMemberAccept = true;
            db.SaveChanges();
            return subscriptionStatus.ToString();
        }
        /// <summary>
        /// member remove company invitation/ exit the membership of a company
        /// </summary>
        /// <param name="companyId"></param
        /// <param name="userId"></param
        /// <returns></returns>
        public string ResetState(long companyId, long userId)
        {
            using var db = new ServiceContext();
            var member = db.Members.Where(i => i.UserId == userId && i.CompanyId == companyId).FirstOrDefault();
            if (member == null) return Constant.InivitationStatus.ERROR_NOT_FOUND.ToString();

            var subscriptionStatus = subscriptionService.CheckOrAddSpecificUsage(ConstantModel.BusinessPackageItem.Member, companyId, -1, addAfterSubscriptionValid: true);
            if (!subscriptionStatus.Equals(Constant.BusinessUsageStatus.OK)) return subscriptionStatus.ToString();

            member.IsActive = false;
            //reset state
            member.IsCompanyAccept = false;
            member.IsAdministrator = false;
            member.IsMemberAccept = false;
            db.SaveChanges();
            return subscriptionStatus.ToString();
        }
    }
}
