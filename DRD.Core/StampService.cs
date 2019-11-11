using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Based.Core.Entity;
using DRD.Domain;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Based.Core;

namespace DRD.Service
{
    public class StampService
    {
        private readonly string _connString;

        //public StampService(string connString)
        //{
        //    _connString = connString;
        //}

        public StampService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public DtoStamp GetById(long id)
        {

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Stamps
                     where c.Id == id
                     select new DtoStamp
                     {
                         Id = c.Id,
                         CompanyId = c.CompanyId,
                         Descr = c.Descr,
                         StampFile = c.StampFile,
                         CreatorId = c.CreatorId,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                     }).FirstOrDefault();

                return result;
            }
        }

        public IEnumerable<DtoStamp> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<DtoStamp> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<DtoStamp> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order, string criteria)
        {

            int skip = pageSize * (page - 1);
            string ordering = "DateCreated desc";

            if (!string.IsNullOrEmpty(order))
                ordering = order;

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new DrdContext(_connString))
            {

                var result =
                (from c in db.Stamps
                 where c.CreatorId == creatorId && (topCriteria == null || tops.All(x => (c.Descr).Contains(x)))
                 select new DtoStamp
                 {
                     Id = c.Id,
                     CompanyId = c.CompanyId,
                     Descr = c.Descr,
                     StampFile = c.StampFile,
                     CreatorId = c.CreatorId,
                     UserId = c.UserId,
                     DateCreated = c.DateCreated,
                 }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService musvr = new MenuService();
                    foreach (DtoStamp dl in result)
                    {
                        dl.Key = musvr.EncryptData(dl.Id);
                    }
                }
                return result;

            }
        }

        public IEnumerable<DtoStampLite> GetLite(long creatorId, string topCriteria, int page, int pageSize, string order, string criteria)
        {

            int skip = pageSize * (page - 1);
            string ordering = "DateCreated desc";

            if (!string.IsNullOrEmpty(order))
                ordering = order;

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new DrdContext(_connString))
            {

                var result =
                (from c in db.Stamps
                 where c.CreatorId == creatorId && (topCriteria == null || tops.All(x => (c.Descr).Contains(x)))
                 select new DtoStampLite
                 {
                     Id = c.Id,
                     Descr = c.Descr,
                     StampFile = c.StampFile,
                     DateCreated = c.DateCreated,
                 }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }


        public long GetLiteAllCount(long creatorId, string topCriteria)
        {
            return GetLiteAllCount(creatorId, topCriteria, null);
        }
        public long GetLiteAllCount(long creatorId, string topCriteria, string criteria)
        {

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Stamps
                     where c.CreatorId == creatorId && (topCriteria == null || tops.All(x => (c.Descr).Contains(x)))
                     select new DtoStamp
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();

                return result;

            }
        }

        public int Save(DtoStamp prod)
        {

            Stamp product;
            int result = 0;

            using (var db = new DrdContext(_connString))
            {
                if (prod.Id != 0)
                    product = db.Stamps.FirstOrDefault(c => c.Id == prod.Id);
                else
                    product = new Stamp();

                product.CompanyId = prod.CompanyId;
                product.Descr = prod.Descr;
                product.StampFile = prod.StampFile;
                product.CreatorId = prod.CreatorId;
                product.UserId = prod.UserId;
                if (prod.Id == 0)
                {
                    product.DateCreated = DateTime.Now;
                    db.Stamps.Add(product);
                }
                else
                    product.DateUpdated = DateTime.Now;

                result = db.SaveChanges();


                return result;
            }

        }

    }
}
