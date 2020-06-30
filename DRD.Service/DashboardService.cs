using DRD.Models.API;
using DRD.Service.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DRD.Service
{
    public class DashboardService
    {
        /// <summary>
        /// GET counter of Rotation divided by status
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="companyId">default value is 0, if you are not specify company id it will search for personal rotation</param>
        /// <returns>the counting result is filled in value from counteRrotation.new </returns>
        public CounterRotation GetActivityCounter(long userId, long companyId = 0)
        {
            var counter = new CounterRotation();
            using var db = new ServiceContext();
            // Personal
            if (companyId == 0)
            {
                var rotations = db.Rotations.Where(c => c.UserId == userId).ToList();
                if (rotations != null)
                {
                    counter.New.InProgress = rotations.Count(c => c.Status == (int)Constant.RotationStatus.In_Progress);
                    counter.New.Completed = rotations.Count(c => c.Status == (int)Constant.RotationStatus.Completed);
                    counter.New.Rejected = rotations.Count(c => c.Status == (int)Constant.RotationStatus.Declined);
                }
                return counter;
            }
            /// EXECUTE ONLY IF COMPANY ID IS FILLED
            CompanyService companyService = new CompanyService();

            var rotationsCompany = db.Rotations.Where(c => c.SubscriptionType == (byte)Constant.SubscriptionType.BUSINESS && c.CompanyId == companyId).ToList();
            if (rotationsCompany  != null)
            {
                counter.New.InProgress = rotationsCompany.Count(c => c.Status == (int)Constant.RotationStatus.In_Progress);
                counter.New.Completed = rotationsCompany.Count(c => c.Status == (int)Constant.RotationStatus.Completed);
                counter.New.Rejected = rotationsCompany.Count(c => c.Status == (int)Constant.RotationStatus.Declined);
            }
            return counter;
        }
        /// <summary>
        /// GET Subscription status limit of storage and other things that has a limit
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="companyId"></param>
        /// <returns>the counting result is filled in value from subscriptionLimit.new</returns>
        public SubscriptionLimit GetCompanySubscriptionLimit(long companyId)
        {
            using (var db = new ServiceContext())
            {
                CompanyService companyService = new CompanyService();
                SubscriptionService subscriptionService = new SubscriptionService();
                var storage = new SubscriptionLimit();
                var storages = subscriptionService.GetActiveBusinessSubscriptionByCompany(companyId: companyId);

                if (storages != null)
                {
                    storage.New.StorageLimit = storages.StorageLimit.Value;
                    storage.New.TotalStorage = storages.TotalStorage.Value;
                }
                return storage;
            }
        }

        // Belom dipake ini bang
        public int SendEmailNotifikasiRotasi(long rotationId, long userId)
        {
            var configGenerator = new AppConfigGenerator();
            var topaz = configGenerator.GetConstant("APPLICATION_NAME")["value"];
            var senderName = configGenerator.GetConstant("EMAIL_USER_DISPLAY")["value"];
            EmailService emailService = new EmailService();

            string body = emailService.CreateHtmlBody(System.Web.HttpContext.Current.Server.MapPath("/doc/emailtemplate/rotationReminder.html"));
            String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
            String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

            var senderEmail = configGenerator.GetConstant("EMAIL_USER")["value"];
            return 0;
        }
        /// <summary>
        /// GET rotations that company has and filtered by tags, the data will contains where is the rotation going on
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="tags"></param>
        /// <param name="skip"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ICollection<RotationDashboard> GetRotationsByCompany(long companyId, ICollection<string> tags, int skip, int pageSize)
        {
            using (var db = new ServiceContext())
            {
                var data = (from rotation in db.Rotations
                            where rotation.CompanyId == companyId
                            orderby rotation.UpdatedAt descending
                            select new RotationDashboard
                            {
                                Id = rotation.Id,
                                Subject = rotation.Name,
                                Status = rotation.Status,
                                DateCreated = rotation.CreatedAt,
                                DateUpdated = rotation.UpdatedAt,
                                DateStarted = rotation.StartedAt,
                                Tags = (from tagitem in rotation.TagItems
                                        join tag in db.Tags on tagitem.TagId equals tag.Id
                                        select tag.Name.ToLower()).ToList(),
                                RotationUsers = (from rtuser in rotation.RotationUsers
                                                 select new RotationDashboard.UserDashboard
                                                 {
                                                     Id = rtuser.User.Id,
                                                     Name = rtuser.User.Name,
                                                     ImageProfile = rtuser.User.ProfileImageFileName
                                                 }).ToList(),
                                Creator = (from user in db.Users
                                           where user.Id == rotation.CreatorId
                                           select new RotationDashboard.UserDashboard
                                           {
                                               Id = user.Id,
                                               Name = user.Name,
                                               ImageProfile = user.ProfileImageFileName
                                           }).FirstOrDefault(),
                                Workflow = new RotationDashboard.WorkflowDashboard
                                {
                                    Id = rotation.Workflow.Id,
                                    Name = rotation.Workflow.Name
                                }
                            });
                if (tags != null)
                    data = data.Where(item => tags.All(itag => item.Tags.Contains(itag.ToLower())));
                if (pageSize > 0 && skip >= 0)
                    data = data.Skip(skip).Take(pageSize);
                var result = data.ToList();
                foreach (RotationDashboard x in result)
                {
                    x.InboxId = db.Inboxes.Where(inbox => inbox.RotationId == x.Id).FirstOrDefault().Id;
                    x.Creator.EncryptedId = Utilities.Encrypt(x.Creator.Id.ToString());
                    foreach (RotationDashboard.UserDashboard y in x.RotationUsers)
                    {
                        var rNode = (from rotationNode in db.RotationNodes
                                     where rotationNode.Rotation.Id == x.Id
                                     && rotationNode.UserId == y.Id
                                     select new RotationNodeInboxData
                                     {
                                         CreatedAt = rotationNode.CreatedAt,
                                         Status = rotationNode.Status
                                     }).FirstOrDefault();
                        if (rNode != null)
                        {
                            y.InboxStatus = rNode.Status;
                            y.InboxTimeStamp = rNode.CreatedAt;
                        }
                        else
                        {
                            y.InboxTimeStamp = DateTime.MaxValue;
                            y.InboxStatus = -99;
                        }
                        y.EncryptedId = Utilities.Encrypt(y.Id.ToString());
                    }
                    x.RotationUsers = x.RotationUsers.OrderBy(i => i.InboxTimeStamp).ToList();
                }
                return result;
            }
        }
        /// <summary>
        /// Function to count all Rotation status of a Company from Models
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public int CountRotationsByCompany(long companyId, ICollection<string> tags)
        {
            using (var db = new ServiceContext())
            {
                var data = (from rotation in db.Rotations
                            where rotation.CompanyId == companyId
                            orderby rotation.UpdatedAt descending
                            select new RotationDashboard
                            {
                                Id = rotation.Id,
                                Subject = rotation.Name,
                                Status = rotation.Status,
                                DateCreated = rotation.CreatedAt,
                                DateUpdated = rotation.UpdatedAt,
                                DateStarted = rotation.StartedAt,
                                Tags = (from tagitem in rotation.TagItems
                                        join tag in db.Tags on tagitem.TagId equals tag.Id
                                        select tag.Name.ToLower()).ToList(),
                                RotationUsers = (from rtuser in rotation.RotationUsers
                                                 select new RotationDashboard.UserDashboard
                                                 {
                                                     Id = rtuser.User.Id,
                                                     Name = rtuser.User.Name,
                                                     ImageProfile = rtuser.User.ProfileImageFileName
                                                 }).ToList(),
                                Creator = (from user in db.Users
                                           where user.Id == rotation.CreatorId
                                           select new RotationDashboard.UserDashboard
                                           {
                                               Id = user.Id,
                                               Name = user.Name,
                                               ImageProfile = user.ProfileImageFileName
                                           }).FirstOrDefault(),
                                Workflow = new RotationDashboard.WorkflowDashboard
                                {
                                    Id = rotation.Workflow.Id,
                                    Name = rotation.Workflow.Name
                                }
                            });
                if (tags != null)
                    data = data.Where(item => tags.All(itag => item.Tags.Contains(itag.ToLower())));
                var result = data.ToList().Count();
                return result;
            }
        }
        /// <summary>
        /// CHECK if user has a company or not
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CheckIsUserHasCompany(long userId)
        {
            UserService helper = new UserService();
            return helper.HasCompany(userId);
        }

    }

    public static class Ext
    {
        private const long OneKb = 1000;
        private const long OneMb = OneKb * 1000;
        private const long OneGb = OneMb * 1000;
        private const long OneTb = OneGb * 1000;

        public static string ToPrettySize(this int value, int decimalPlaces = 2)
        {
            return ((long)value).ToPrettySize(decimalPlaces);
        }

        public static string ToPrettySize(this long value, int decimalPlaces = 2)
        {
            var asTb = Math.Round((double)value / OneTb, decimalPlaces);
            var asGb = Math.Round((double)value / OneGb, decimalPlaces);
            var asMb = Math.Round((double)value / OneMb, decimalPlaces);
            var asKb = Math.Round((double)value / OneKb, decimalPlaces);
            string chosenValue = asTb > 1 ? string.Format("{0}Tb", asTb)
                : asGb > 1 ? string.Format("{0}Gb", asGb)
                : asMb > 1 ? string.Format("{0}Mb", asMb)
                : asKb > 1 ? string.Format("{0}Kb", asKb)
                : string.Format("{0}B", Math.Round((double)value, decimalPlaces));
            return chosenValue;
        }
    }
}