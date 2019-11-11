using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Based.Core.Entity;
using DRD.Domain;
using System.Configuration;
using System.Based.Core;

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
            _connString = ConfigConstant.CONSTRING;
        }
        public DashboardService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        //public decimal GetDepositBalance(long memberId)
        //{
        //    using (var db = new DrdContext(_connString))
        //    {
        //        // get topup
        //        var masuk = db.MemberTopupDeposits.Where(c => c.MemberId == memberId && c.PaymentStatus.Equals("02")).ToList().Sum(c => c.Amount);
        //        decimal keluar = 0;

        //        // get rotation price
        //        keluar = db.Rotations.Where(c => c.CreatorId == memberId && !("00").Contains(c.Status)).ToList().Sum(c => c.Price);

        //        return masuk - keluar;
        //    }
        //}

        public JsonCounter GetActivityCounter(long memberId, JsonCounter counter)
        {
            using (var db = new DrdContext(_connString))
            {
                counter.Old.UnReadChat = counter.New.UnReadChat;
                counter.Old.Inbox = counter.New.Inbox;
                counter.Old.Altered = counter.New.Altered;
                counter.Old.Revised = counter.New.Revised;
                counter.Old.Pending = counter.New.Pending;
                counter.Old.Signed = counter.New.Signed;

                counter.Old.Rotation = counter.New.Rotation;
                counter.Old.InProgress = counter.New.InProgress;
                counter.Old.Completed = counter.New.Completed;
                counter.Old.Declined = counter.New.Declined;

                counter.Old.DepositBalance = counter.New.DepositBalance;
                counter.Old.DrDrive = counter.New.DrDrive;
                counter.Old.Contact = counter.New.Contact;

                var rotnodes = db.RotationNodes.Where(c => c.MemberId == memberId).ToList();
                if (rotnodes != null)
                {
                    counter.New.Inbox = rotnodes.Count(c => c.Status.Equals("00"));
                    counter.New.Altered = rotnodes.Count(c => c.Status.Equals("06"));
                    counter.New.Revised = rotnodes.Count(c => c.Status.Equals("05"));
                    counter.New.Pending = rotnodes.Count(c => c.Status.Equals("02"));
                    counter.New.Signed = rotnodes.Count(c => c.Status.Equals("03"));

                    long[] Ids = (from c in rotnodes select c.RotationId).ToArray();
                    var rot = db.Rotations.Where(c => Ids.Contains(c.Id) || c.MemberId == memberId).ToList();
                    //var rot = db.Rotations.Where(c => c.CreatorId == memberId).ToList();
                    counter.New.Rotation = rot.Count(c => c.Status.Equals("00") && c.CreatorId == memberId);
                    counter.New.InProgress = rot.Count(c => c.Status.Equals("01"));
                    counter.New.Completed = rot.Count(c => c.Status.Equals("90"));
                    counter.New.Declined = rot.Count(c => c.Status.Equals("98"));

                }
                MemberDepositTrxService xsvr = new MemberDepositTrxService();
                counter.New.DepositBalance = xsvr.GetDepositBalance(memberId);

                DrDriveService drvsrv = new DrDriveService();
                JsonDrDriveCount drvcx = drvsrv.GetCounting(memberId);
                counter.New.DrDrive = drvcx.TotalStorageUsage;
                counter.New.DrDriveDesc = Ext.ToPrettySize((long) drvcx.TotalStorageUsage);

                MemberService memsvr = new MemberService();
                long cx = memsvr.GetInvitedLiteAllCount(memberId, null, null);
                counter.New.Contact = (int)cx;

                DocumentService docsvr = new DocumentService();
                long cxdoc = docsvr.GetLiteAllCount(memberId, null);
                counter.New.Document = (int)cxdoc;

                MessageService msgsvr = new MessageService();
                long cxchat = msgsvr.GetCount(memberId).Unread;
                counter.New.UnReadChat = (int)cxchat;
                return counter;

            }
        }

        /// <summary>
        /// for smartphone
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public JsonDashboard GetCount(long memberId)
        {
            JsonDashboard counter = new JsonDashboard();
            using (var db = new DrdContext(_connString))
            {
                var rotnodes = db.RotationNodes.Where(c => c.MemberId == memberId).ToList();
                if (rotnodes != null)
                {
                    counter.Inbox = rotnodes.Count(c => c.Status.Equals("00"));
                    counter.Altered = rotnodes.Count(c => c.Status.Equals("06"));
                    counter.Revised = rotnodes.Count(c => c.Status.Equals("05"));
                    counter.Pending = rotnodes.Count(c => c.Status.Equals("02"));
                    counter.Signed = rotnodes.Count(c => c.Status.Equals("03"));

                    long[] Ids = (from c in rotnodes select c.RotationId).ToArray();
                    if (Ids.Length > 0)
                        Ids = Ids.Distinct().ToArray();
                    var rot = db.Rotations.Where(c => Ids.Contains(c.Id) || c.MemberId == memberId).ToList();

                    counter.Rotation = rot.Count(c => c.Status.Equals("00"));
                    counter.InProgress = rot.Count(c => c.Status.Equals("01"));
                    counter.Completed = rot.Count(c => c.Status.Equals("90"));
                    counter.Declined = rot.Count(c => c.Status.Equals("98"));
                }
                // get chat unread
                MessageService msgsvr = new MessageService();
                counter.UnreadChat = msgsvr.GetCount(memberId).Unread;
                // get deposit balance
                MemberDepositTrxService xsvr = new MemberDepositTrxService();
                counter.DepositBalance = xsvr.GetDepositBalance(memberId);
                // invitation
                MemberService memsvr = new MemberService();
                memsvr.GetInvitationCount(memberId, ref counter);

                return counter;
            }
        }

        public bool Compare(long memberId, JsonDashboard db2)
        {
            JsonDashboard db = GetCount(memberId);
            return (
                db.Rotation == db2.Rotation &&
                db.Inbox == db2.Inbox &&
                db.Altered == db2.Altered &&
                db.Revised == db2.Revised &&
                db.InProgress == db2.InProgress &&
                db.Pending == db2.Pending &&
                db.Signed == db2.Signed &&
                db.Declined == db2.Declined &&
                db.Completed == db2.Completed &&
                db.UnreadChat == db2.UnreadChat &&
                db.InviteAccepted == db2.InviteAccepted &&
                db.Invitation == db2.Invitation &&
                db.DepositBalance == db2.DepositBalance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public JsonDashboardAdmin GetAdminCount()
        {
            JsonDashboardAdmin dashboard = new JsonDashboardAdmin();

            MemberTopupDepositService mtdsvr = new MemberTopupDepositService();
            mtdsvr.GetCount(ref dashboard);

            return dashboard;
        }

        public bool AdminCompare(JsonDashboardAdmin dash)
        {
            JsonDashboardAdmin newdashb = GetAdminCount();

            return (dash.MemberTopupPending == newdashb.MemberTopupPending
                && dash.MemberTopupConfirmation == newdashb.MemberTopupConfirmation
                && dash.MemberTopupConfirmed == newdashb.MemberTopupConfirmed
                && dash.MemberTopupNotConfirmed == newdashb.MemberTopupNotConfirmed);
        }
    }

    public static class Ext
    {
        private const long OneKb = 1024;
        private const long OneMb = OneKb * 1024;
        private const long OneGb = OneMb * 1024;
        private const long OneTb = OneGb * 1024;

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
