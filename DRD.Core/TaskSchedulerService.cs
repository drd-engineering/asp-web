using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Based.Core.Entity;
using DRD.Domain;
using System.Configuration;
using Newtonsoft.Json;
using System.Based.Core;

namespace DRD.Service
{
    public class TaskSchedulerService
    {
        private readonly string _connString;

        public TaskSchedulerService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        /// <summary>
        /// auto proces for ALTER
        /// </summary>
        /// <returns></returns>
        public int ProcessAlterNode()
        {
            using (var db = new DrdContext(_connString))
            {
                var rns = db.RotationNodes.Where(c => c.Status.Equals("00") && c.WorkflowNode.WorkflowNodeLinks_WorkflowNodeId.Any(x => x.Symbol.Code.Equals("ALTER"))).ToList();
                if (rns == null)
                    return 0;

                RotationService rtsvr = new RotationService();
                int cx = 0;
                foreach (RotationNode rn in rns)
                {
                    var wfl = db.WorkflowNodeLinks.FirstOrDefault(c => c.WorkflowNodeId == rn.WorkflowNodeId && c.Symbol.Code.Equals("ALTER"));
                    DateTime dueData = rn.DateCreated;
                    if (wfl.Operator.Equals("HOUR"))
                        dueData = dueData.AddHours(int.Parse(wfl.Value));
                    else if (wfl.Operator.Equals("DAY"))
                        dueData = dueData.AddDays(int.Parse(wfl.Value));

                    if (dueData <= DateTime.Now)
                    {
                        JsonProcessActivity param = new JsonProcessActivity();
                        param.RotationNodeId = rn.Id;
                        rtsvr.ProcessActivity(param, ConfigConstant.EnumActivityAction.ALTER);
                        cx++;
                    }
                }
                return cx;
            }
        }

        /// <summary>
        /// auto expired payment 
        /// </summary>
        /// <returns></returns>
        public int ProcessExpiredPayment()
        {
            using (var db = new DrdContext(_connString))
            {
                var pays = db.MemberTopupDeposits.Where(c => c.PaymentStatus.Equals("00")).ToList();
                if (pays == null)
                    return 0;

                int cx = 0;
                foreach (MemberTopupDeposit topup in pays)
                {
                    DateTime dueData = topup.DateCreated.AddHours(2);
                    if (dueData <= DateTime.Now)
                    {
                        topup.PaymentStatus = "98";
                        cx++;
                    }
                }
                if (cx > 0)
                    db.SaveChanges();
                return cx;
            }
        }

        /// <summary>
        /// auto renew member plan 
        /// </summary>
        /// <returns></returns>
        public int ProcessRenewMemberPlan()
        {
            using (var db = new DrdContext(_connString))
            {
                var plans = db.MemberPlans.Where(c => c.PackageExpiryDay > 0 && c.IsDefault).ToList();
                if (plans == null)
                    return 0;

                MemberService mpsvr = new MemberService();
                int cx = 0;
                foreach (MemberPlan plan in plans)
                {
                    DateTime dueData = plan.DateCreated.AddDays(plan.PackageExpiryDay);
                    if (dueData <= DateTime.Now)
                    {
                        if (mpsvr.UpgradePlan(plan.MemberId, plan.SubscriptTypeId) > 0)
                            cx++;
                    }
                }
                if (cx > 0)
                    db.SaveChanges();
                return cx;
            }
        }

    }
}
