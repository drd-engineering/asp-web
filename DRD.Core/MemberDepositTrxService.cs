using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Based.Core.Entity;
using DRD.Domain;
using System.Based.Core;

namespace DRD.Core
{
    public class MemberDepositTrxService : IMemberDepositTrxService
    {
        private readonly string _connString;

        public MemberDepositTrxService()
        {
            _connString = ConfigConstant.CONSTRING;
        }
        public MemberDepositTrxService(string connString)
        {
            _connString = connString;
        }

        public IEnumerable<DtoMemberDepositTrx> GetByQuery(string query, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "DateCreated desc";

            if (order != null)
                ordering = order;

            if (criteria == null)
                criteria = query;
            else
                criteria = query + " and " + criteria;

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.MemberDepositTrxes
                     select new DtoMemberDepositTrx
                     {
                         Id = c.Id,
                         TrxNo = c.TrxNo,
                         TrxDate = c.TrxDate,
                         TrxType = c.TrxType,
                         TrxId = c.TrxId,
                         Descr = c.Descr,
                         MemberId = c.MemberId,
                         Amount = c.Amount,
                         DbCr = c.DbCr,
                         DateCreated = c.DateCreated,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                //if (result != null)
                //    result = result.OrderBy("DateCreated").ToList();

                return result;
            }
        }
        public long GetByQueryCount(long memberId)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.MemberDepositTrxes
                     where c.MemberId == memberId
                     select new DtoMemberDepositTrx
                     {
                         Id = c.Id,
                     }).Count();

                return result;
            }
        }

        public DtoMemberDepositTrx GetById(long id)
        {
            var result = GetByQuery("Id=" + id, 1, 1, null, null).FirstOrDefault();
            return result;
        }

        public IEnumerable<DtoMemberDepositTrx> GetById(long MemberId, int page, int pageSize, string order, string criteria)
        {
            var result = GetByQuery("MemberId=" + MemberId, page, pageSize, order, criteria);
            return result;
        }

        public int Save(DtoMemberDepositTrx trx)
        {
            MemberDepositTrx data = new MemberDepositTrx();
            using (var db = new DrdContext(_connString))
            {
                data.TrxNo = trx.TrxNo;
                data.TrxDate = trx.TrxDate;
                data.TrxType = trx.TrxType;
                data.TrxId = trx.TrxId;
                data.Descr = trx.Descr;
                data.MemberId = trx.MemberId;
                data.Amount = trx.Amount;
                data.DbCr = trx.DbCr;
                data.DateCreated = DateTime.Now;
                db.MemberDepositTrxes.Add(data);
                return db.SaveChanges();
            }
        }


        //public decimal GetCount(int memberId, ref JsonDashboard dashb)
        //{
        //    using (var db = new DrdContext(_connString))
        //    {
        //        dashb.DepositBalance = GetDepositBalance(memberId);
        //    }

        //    return dashb.DepositBalance;
        //}

        public decimal GetDepositBalance(long memberId)
        {
            using (var db = new DrdContext(_connString))
            {
                var saldo = db.MemberDepositTrxes.Where(c => c.MemberId == memberId).ToList().Sum(c => (c.DbCr == 0 ? c.Amount : -c.Amount));
                return saldo;
            }

        }
    }
}
