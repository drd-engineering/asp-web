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
    public class VoucherService
    {
        private readonly string _connString;

        public VoucherService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public VoucherService(string connString)
        {
            _connString = connString;
        }

        public IEnumerable<DtoVoucher> GetAll()
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Vouchers
                     orderby c.Number
                     select new DtoVoucher
                     {
                         Id = c.Id,
                         Number = c.Number,
                         Nominal = c.Nominal,
                         Price = c.Price,
                         VoucherType = c.VoucherType,
                         TrxId = c.TrxId,
                         TrxType = c.TrxType,
                         TrxUserId = c.TrxUserId,
                         DateUsed = c.DateUsed,
                         DateCreated = c.DateCreated,
                     }).ToList();

                return result;
            }
        }

        public DtoVoucher GetById(int id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Vouchers
                     where c.Id == id
                     select new DtoVoucher
                     {
                         Id = c.Id,
                         Number = c.Number,
                         Nominal = c.Nominal,
                         Price = c.Price,
                         VoucherType = c.VoucherType,
                         TrxId = c.TrxId,
                         TrxType = c.TrxType,
                         TrxUserId = c.TrxUserId,
                         DateUsed = c.DateUsed,
                         DateCreated = c.DateCreated,
                     }).FirstOrDefault();

                return result;
            }
        }

        public DtoVoucher GetByNumber(string number)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Vouchers
                     where c.Number == number
                     select new DtoVoucher
                     {
                         Id = c.Id,
                         Number = c.Number,
                         Nominal = c.Nominal,
                         Price = c.Price,
                         VoucherType = c.VoucherType,
                         TrxId = c.TrxId,
                         TrxType = c.TrxType,
                         TrxUserId = c.TrxUserId,
                         DateUsed = c.DateUsed,
                         DateCreated = c.DateCreated,
                     }).FirstOrDefault();

                return result;
            }
        }

        public DtoVoucher Save(DtoVoucher vc)
        {
            Voucher data = new Voucher();
            using (var db = new DrdContext(_connString))
            {
                data.Number = vc.Number;
                data.Nominal = vc.Nominal;
                data.Price = vc.Price;
                data.VoucherType = vc.VoucherType;
                data.DateCreated = DateTime.Now;

                db.Vouchers.Add(data);
                var result = db.SaveChanges();
                vc.Id = data.Id;
                vc.DateCreated = data.DateCreated;
                return vc;
            }
        }

        public int SetBooking(long id)
        {
            using (var db = new DrdContext(_connString))
            {
                var data = db.Vouchers.FirstOrDefault(c => c.Id == id);
                if (data == null) return 0;

                if (data.DateUsed != null)
                    return -1;

                data.DateUsed = DateTime.Now;

                var result = db.SaveChanges();
                return result;
            }
        }

        public int SetUsed(DtoVoucher vc)
        {
            using (var db = new DrdContext(_connString))
            {
                var data = db.Vouchers.FirstOrDefault(c => c.Id == vc.Id);
                if (data == null) return 0;

                //if (data.DateUsed != null)
                //    return -1;

                data.TrxId = vc.TrxId;
                data.TrxType = vc.TrxType;
                data.TrxUserId = vc.TrxUserId;
                data.DateUsed = DateTime.Now;

                var result = db.SaveChanges();
                return result;
            }
        }

        public int Generate(int qty, int type, int nominal, int price)
        {
            ApplConfigService appsvr = new ApplConfigService();

            using (var db = new DrdContext(_connString))
            {
                for (int x = 0; x < qty; x++)
                {
                    long i = 1;
                    foreach (byte b in Guid.NewGuid().ToByteArray())
                    {
                        i *= ((int)b + 1);
                    }

                    Voucher vcr = new Voucher();
                    vcr.Number = appsvr.GenerateUniqueKey();
                    vcr.Nominal = nominal;
                    vcr.Price = price;
                    vcr.VoucherType = type;
                    vcr.DateCreated = DateTime.Now;
                    db.Vouchers.Add(vcr);

                }
                var result = db.SaveChanges();
                return result;
            }
        }

    }

}
