using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using DRD.Domain;
using System.Based.Core.Entity;
using System.Data.Entity.Infrastructure;
using System.Based.Core;

namespace DRD.Core
{
    public class VoucherGeneratorService
    {
        private readonly string _connString;

        public VoucherGeneratorService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public VoucherGeneratorService(string connString)
        {
            _connString = connString;
        }

        public IEnumerable<DtoVoucherGenerator> GetAll()
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.VoucherGenerators
                     orderby c.DateCreated descending
                     select new DtoVoucherGenerator
                     {
                         Id = c.Id,
                         Number = c.Number,
                         Nominal = c.Nominal,
                         Price = c.Price,
                         VoucherType = c.VoucherType,
                         Quantity = c.Quantity,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                     }).ToList();

                return result;
            }
        }

        public IEnumerable<DtoVoucherGenerator> GetAllPagging(int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "DateCreated desc";

            if (order != null)
                ordering = order;

            if (criteria == null)
                criteria = "1=1";

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.VoucherGenerators
                     orderby c.DateCreated descending
                     select new DtoVoucherGenerator
                     {
                         Id = c.Id,
                         Number = c.Number,
                         Nominal = c.Nominal,
                         Price = c.Price,
                         VoucherType = c.VoucherType,
                         Quantity = c.Quantity,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }

        public DtoVoucherGenerator GetById(long id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.VoucherGenerators
                     where c.Id == id
                     select new DtoVoucherGenerator
                     {
                         Id = c.Id,
                         Number = c.Number,
                         Nominal = c.Nominal,
                         Price = c.Price,
                         VoucherType = c.VoucherType,
                         Quantity = c.Quantity,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                     }).FirstOrDefault();

                return result;
            }
        }

        public DtoVoucherGenerator GetByNumber(string number)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.VoucherGenerators
                     where c.Number == number
                     select new DtoVoucherGenerator
                     {
                         Id = c.Id,
                         Number = c.Number,
                         Nominal = c.Nominal,
                         Price = c.Price,
                         VoucherType = c.VoucherType,
                         Quantity = c.Quantity,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                     }).FirstOrDefault();

                return result;
            }
        }

        public int Save(DtoVoucherGenerator vc)
        {
            VoucherGenerator data = new VoucherGenerator();
            ApplConfigService appl = new ApplConfigService();
            using (var db = new DrdContext(_connString))
            {

                for (int i = 0; i < ConfigConstant.LOOP_TRY_SAVE; i++)
                {
                    try
                    {
                        data.Number = appl.GenerateNumber("VGEN_NO", "VoucherGenerator", "Number", db.VoucherGenerators);
                        data.Nominal = vc.Nominal;
                        data.Price = vc.Price;
                        data.Quantity = vc.Quantity;
                        data.VoucherType = vc.VoucherType;
                        data.UserId = vc.UserId;
                        data.DateCreated = DateTime.Now;

                        db.VoucherGenerators.Add(data);
                        var result = db.SaveChanges();

                        VoucherService vcsvr = new VoucherService();
                        vcsvr.Generate(vc.Quantity, vc.VoucherType, (int)vc.Nominal, (int)vc.Price);

                        return result;

                    }
                    catch (DbUpdateException x)
                    {
                        if (i > ConfigConstant.LOOP_TRY_SAVE_THROW)
                            throw new Exception(x.Message);
                    }
                }
            }
            return 0;

        }


        public int GetCount()
        {
            using (var db = new DrdContext(_connString))
            {
                DateTime today = DateTime.Today;
                var result = db.VoucherGenerators.Where(c => c.DateCreated >= today);
                int data = 0;
                if (result != null)
                    data = result.Sum(c => c.Quantity);
                return data;
            }
        }

    }

}
