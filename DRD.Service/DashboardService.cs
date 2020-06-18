using DRD.Models.View;
using DRD.Service.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DRD.Service
{
    public class DashboardService
    {
        private readonly string _connString;
        private string _appZoneAccess;

        public DashboardService(string appZoneAccess, string connString)
        {
            _appZoneAccess = appZoneAccess;
            _connString = connString;
        }

        public DashboardService(string appZoneAccess)
        {
            _appZoneAccess = appZoneAccess;
            _connString = Constant.CONSTRING;
        }

        public DashboardService()
        {
            _connString = Constant.CONSTRING;
        }

        public CounterItem GetActivityCounter(long memberId, CounterItem counter, long companyId)
        {
            using (var db = new ServiceContext())
            {
                CompanyService companyService = new CompanyService();
                SubscriptionService subscriptionService = new SubscriptionService();

                counter.Old.InProgress = counter.New.InProgress;
                counter.Old.Completed = counter.New.Completed;
                counter.Old.StorageLimit = counter.New.StorageLimit;
                counter.Old.TotalStorage = counter.New.TotalStorage;

                var rotation = db.Rotations.Where(c => c.SubscriptionType == (byte)Constant.SubscriptionType.BUSINESS && c.SubscriptionOf == companyId).ToList();
                var storages = subscriptionService.GetActiveBusinessSubscriptionByCompany(companyId: companyId);

                if (rotation != null)
                {
                    counter.New.InProgress = rotation.Count(c => c.Status.Equals(Constant.RotationStatus.In_Progress));
                    counter.New.Completed = rotation.Count(c => c.Status.Equals(Constant.RotationStatus.Completed));
                }
                if (storages != null)
                {
                    counter.New.StorageLimit = storages.StorageLimit.Value;
                    counter.New.TotalStorage = storages.TotalStorage.Value;
                }
                return counter;
            }
        }

        public CounterItem GetActivityCounter(long memberId, CounterItem counter)
        {
            using (var db = new ServiceContext())
            {
                counter.Old.InProgress = counter.New.InProgress;
                counter.Old.Completed = counter.New.Completed;

                var rotationNodes = db.RotationNodes.Where(c => c.MemberId == memberId).ToList();
                if (rotationNodes != null)
                {
                    long[] Ids = (from c in rotationNodes select c.Rotation.Id).ToArray();
                    var rot = db.Rotations.Where(c => Ids.Contains(c.Id) || c.UserId == memberId).ToList();

                    counter.New.InProgress = rot.Count(c => c.Status == 1);
                    counter.New.Completed = rot.Count(c => c.Status == 90);
                }
                return counter;
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

            try
            {
                var task = emailService.Send(senderEmail, senderName + "Administrator", "franna.jaya@gmail.com", senderName + " User Registration", body, false, new string[] { });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in RetryIfBusy(): {0}",
                        ex.ToString());
            }
            return 1;
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