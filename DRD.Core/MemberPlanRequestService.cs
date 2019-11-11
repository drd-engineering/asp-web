using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Based.Core.Entity;
using DRD.Domain;
using System.Configuration;
using DRD.Service;
using System.Based.Core;

namespace DRD.Service
{
    public class MemberPlanRequestService
    {
        private readonly string _connString;

        public MemberPlanRequestService(string connString)
        {
            _connString = connString;
        }

        public MemberPlanRequestService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public DtoMemberPlanRequest GetById(int id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.MemberPlanRequests
                     where c.Id == id
                     select new DtoMemberPlanRequest
                     {
                         Id = c.Id,
                         MemberId = c.MemberId,
                         SubscriptTypeId = c.SubscriptTypeId,
                         RotationCount = c.RotationCount,
                         FlowActivityCount = c.FlowActivityCount,
                         StorageSize = c.StorageSize,
                         ExpiryDocDay = c.ExpiryDocDay,
                         Status = c.Status,
                         DateCreated = c.DateCreated,
                     }).FirstOrDefault();

                return result;
            }

        }
        public DtoMemberPlanRequest GetByMemberId(long id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.MemberPlanRequests
                     where c.MemberId == id && c.Status.Equals("00")
                     select new DtoMemberPlanRequest
                     {
                         Id = c.Id,
                         MemberId = c.MemberId,
                         SubscriptTypeId = c.SubscriptTypeId,
                         RotationCount = c.RotationCount,
                         FlowActivityCount = c.FlowActivityCount,
                         StorageSize = c.StorageSize,
                         ExpiryDocDay = c.ExpiryDocDay,
                         Status = c.Status,
                         DateCreated = c.DateCreated,
                     }).FirstOrDefault();

                return result;
            }

        }

        public long Save(long memberId, int subscriptTypeId)
        {
            using (var db = new DrdContext(_connString))
            {
                var subscr = db.SubscriptTypes.FirstOrDefault(c => c.Id == subscriptTypeId);
                var plan = new MemberPlanRequest();
                plan.MemberId = memberId;
                plan.SubscriptTypeId = subscriptTypeId;
                plan.RotationCount = 0;
                plan.FlowActivityCount = 0;
                plan.StorageSize = 0;
                plan.ExpiryDocDay = 0;
                plan.DateCreated = DateTime.Now;
                plan.Status = "00";
                db.MemberPlanRequests.Add(plan);
                db.SaveChanges();
                return plan.Id;
            }
        }

    }
}
