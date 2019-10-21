using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Domain;
using System.Based.Core.Entity;
using System.Based.Core;

namespace DRD.Core
{
    public class MemberHitLogService
    {
        private readonly string _connString;

        public MemberHitLogService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public MemberHitLogService(string connString)
        {
            _connString = connString;
        }

        public int Save(long memberId, long dataId, ConfigConstant.enumDataHit hitType)
        {
            MemberHitLog data = new MemberHitLog();
            using (var db = new DrdContext(_connString))
            {
                data.MemberId = memberId;
                data.DataHitId = dataId;
                data.DataHitType = (int)hitType;
                data.DateCreated = DateTime.Now;

                db.MemberHitLogs.Add(data);
                var result = db.SaveChanges();
                return result;
            }
        }

    }
}
