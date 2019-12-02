using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using DRD.Service.Context;
using DRD.Models.View.Dashboard;
using DRD.Service;

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
        public CounterItem GetActivityCounter(long memberId, CounterItem counter, long CompanyId)
        {
            using (var db = new ServiceContext())
            {
                counter.Old.InProgress = counter.New.InProgress;
                counter.Old.Completed = counter.New.Completed;
                
                var rotation = db.Rotations.Where(c => c.SubscriptionType == (byte)Constant.SubscriptionType.BUSINESS && c.SubscriptionOf == CompanyId).ToList();
                if (rotation != null)
                {
                    counter.New.InProgress = rotation.Count(c => c.Status.Equals(Constant.RotationStatus.In_Progress));
                    counter.New.Completed = rotation.Count(c => c.Status.Equals(Constant.RotationStatus.Completed));
                }
                return counter;
            }
        }

        public CounterItem GetActivityCounter(long memberId, CounterItem counter)
        {
            using (var db = new ServiceContext())
            {
                counter.Old.Inbox = counter.New.Inbox;
                counter.Old.Altered = counter.New.Altered;
                counter.Old.Revised = counter.New.Revised;
                counter.Old.Pending = counter.New.Pending;
                counter.Old.Signed = counter.New.Signed;

                counter.Old.Rotation = counter.New.Rotation;
                counter.Old.InProgress = counter.New.InProgress;
                counter.Old.Completed = counter.New.Completed;
                counter.Old.Declined = counter.New.Declined;
                counter.Old.Contact = counter.New.Contact;

                var rotationNodes = db.RotationNodes.Where(c => c.MemberId == memberId).ToList();
                if (rotationNodes != null)
                {
                    counter.New.Inbox = rotationNodes.Count(c => c.Status.Equals("00"));
                    counter.New.Altered = rotationNodes.Count(c => c.Status.Equals("06"));
                    counter.New.Revised = rotationNodes.Count(c => c.Status.Equals("05"));
                    counter.New.Pending = rotationNodes.Count(c => c.Status.Equals("02"));
                    counter.New.Signed = rotationNodes.Count(c => c.Status.Equals("03"));

                    long[] Ids = (from c in rotationNodes select c.Rotation.Id).ToArray();
                    var rot = db.Rotations.Where(c => Ids.Contains(c.Id) || c.Member.Id == memberId).ToList();
                    //var rot = db.Rotations.Where(c => c.CreatorId == memberId).ToList();
                    counter.New.Rotation = rot.Count(c => c.Status.Equals("00") && c.CreatorId == memberId);
                    counter.New.InProgress = rot.Count(c => c.Status.Equals("01"));
                    counter.New.Completed = rot.Count(c => c.Status.Equals("90"));
                    counter.New.Declined = rot.Count(c => c.Status.Equals("98"));

                }


                return counter;

            }
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
