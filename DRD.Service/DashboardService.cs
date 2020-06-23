using DRD.Models.API;
using DRD.Service.Context;
using System;
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
        /// <summary>
        /// Obtain counter of Rotation divided by status and older counter
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="counter"></param>
        /// <param name="companyId">default value is 0, if you are not specify company id it will search for personal rotation</param>
        /// <returns></returns>
        public CounterRotation GetActivityCounter(long userId, CounterRotation counter, long companyId = 0)
        {
            using (var db = new ServiceContext())
            {
                // Personal
                if (companyId < 1)
                {
                    counter.Old.InProgress = counter.New.InProgress;
                    counter.Old.Completed = counter.New.Completed;
                    counter.Old.Rejected = counter.New.Rejected;

                    var rotations = db.Rotations.Where(c => c.UserId == userId).ToList();
                    if (rotations != null)
                    {
                        counter.New.InProgress = rotations.Count(c => c.Status == (int)Constant.RotationStatus.In_Progress);
                        counter.New.Completed = rotations.Count(c => c.Status == (int)Constant.RotationStatus.Completed);
                        counter.New.Rejected = rotations.Count(c => c.Status == (int)Constant.RotationStatus.Declined);
                    }
                    return counter;
                }
                /// EXECUTE ONLY IF COMPANY ID IS > 0
                CompanyService companyService = new CompanyService();

                counter.Old.InProgress = counter.New.InProgress;
                counter.Old.Completed = counter.New.Completed;
                counter.Old.Rejected = counter.New.Rejected;

                var rotationsCompany = db.Rotations.Where(c => c.SubscriptionType == (byte)Constant.SubscriptionType.BUSINESS && c.SubscriptionOf == companyId).ToList();
                
                if (rotationsCompany  != null)
                {
                    counter.New.InProgress = rotationsCompany.Count(c => c.Status == (int)Constant.RotationStatus.In_Progress);
                    counter.New.Completed = rotationsCompany.Count(c => c.Status == (int)Constant.RotationStatus.Completed);
                    counter.New.Rejected = rotationsCompany.Count(c => c.Status == (int)Constant.RotationStatus.Declined);
                }
                return counter;
            }
        }
        /// <summary>
        /// Obtain Subscription status limit of storage and other things that has a limit
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public SubscriptionLimit GetCompanySubscriptionLimit(SubscriptionLimit storage, long companyId)
        {
            using (var db = new ServiceContext())
            {
                CompanyService companyService = new CompanyService();
                SubscriptionService subscriptionService = new SubscriptionService();

                storage.Old.StorageLimit = storage.New.StorageLimit;
                storage.Old.TotalStorage = storage.New.TotalStorage;

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