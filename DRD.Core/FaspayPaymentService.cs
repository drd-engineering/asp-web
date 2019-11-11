using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Domain;
using System.Based.Core.Entity;
using System.Based.Core;

namespace DRD.Service
{
    public class FaspayPaymentService
    {
        private readonly string _connString;

        public FaspayPaymentService(string connString)
        {
            _connString = connString;
        }

        public FaspayPaymentService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public IEnumerable<JsonFaspayPayment> GetAll()
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.FaspayPayments
                     orderby c.DateCreated
                     select new JsonFaspayPayment
                     {
                         Id = c.Id,
                         PayId = c.PayId,
                         PayType = c.PayType,
                         TrxId = c.TrxId,
                         TrxNo = c.TrxNo,
                         TrxType = c.TrxType,
                         DateCreated = c.DateCreated,
                     }).ToList();

                return result;
            }
        }

        public JsonFaspayPayment GetById(long id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.FaspayPayments
                     where c.Id == id
                     select new JsonFaspayPayment
                     {
                         Id = c.Id,
                         PayId = c.PayId,
                         PayType = c.PayType,
                         TrxId = c.TrxId,
                         TrxNo = c.TrxNo,
                         TrxType = c.TrxType,
                         DateCreated = c.DateCreated,
                     }).FirstOrDefault();

                return result;
            }
        }

        public JsonFaspayPayment GetByPayId(long payId)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.FaspayPayments
                     where c.PayId == payId
                     select new JsonFaspayPayment
                     {
                         Id = c.Id,
                         PayId = c.PayId,
                         PayType = c.PayType,
                         TrxId = c.TrxId,
                         TrxNo = c.TrxNo,
                         TrxType = c.TrxType,
                         DateCreated = c.DateCreated,
                     }).FirstOrDefault();

                return result;
            }
        }

        public JsonFaspayPayment Save(JsonFaspayPayment faspayment)
        {
            FaspayPayment data = new FaspayPayment();
            using (var db = new DrdContext(_connString))
            {
                data.PayId = long.Parse(DateTime.Now.ToString("yyMMddHHmmssfffffff"));
                data.PayType = faspayment.PayType;
                data.TrxId = faspayment.TrxId;
                data.TrxNo = faspayment.TrxNo;
                data.TrxType= faspayment.TrxType;
                data.DateCreated = DateTime.Now;

                db.FaspayPayments.Add(data);
                var result = db.SaveChanges();
                faspayment.PayId = data.PayId;
                faspayment.Id = data.Id;
                faspayment.DateCreated = data.DateCreated;
                return faspayment;
            }
        }
    }
}
