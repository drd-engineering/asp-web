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

namespace DRD.Core
{
    public class PodCastService
    {
        private readonly string _connString;

        public PodCastService(string connString)
        {
            _connString = connString;
        }

        public PodCastService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public DtoPodCast GetById(long id)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.PodCasts
                     where c.Id == id
                     select new DtoPodCast
                     {
                         Id = c.Id,
                         Title = c.Title,
                         Descr = c.Descr,
                         Duration = c.Duration,
                         AudioFileName = c.AudioFileName,
                         FileNameOri = c.FileNameOri,
                         Image = c.Image,
                         IsActive = c.IsActive,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         CreatorId = c.CreatorId,
                     }).FirstOrDefault();

                return result;
            }
        }

        public DtoPodCast GetById(long Id, long memberId)
        {
            MemberHitLogService log = new MemberHitLogService();
            log.Save(memberId, Id, ConfigConstant.enumDataHit.PODCAST);

            return GetById(Id);
        }

        public IEnumerable<DtoPodCastLite> GetLiteByTopCriteria(string topCriteria, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "DateCreated desc";

            if (order != null)
                ordering = order;

            if (criteria == null)
                criteria = "1=1";

            string[] tops = new string[] { };
            if (topCriteria != null)
                tops = topCriteria.Split(' ');

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.PodCasts
                     where c.IsActive && (topCriteria == null || tops.All(x => c.Title.Contains(x)))
                     select new DtoPodCastLite
                     {
                         Id = c.Id,
                         Title = c.Title,
                         Duration = c.Duration,
                         Image = c.Image,
                         DateCreated = c.DateCreated,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }

        public IEnumerable<DtoPodCastLite> GetLiteByTopCriteria(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "DateCreated desc";

            if (order != null)
                ordering = order;

            if (criteria == null)
                criteria = "1=1";

            string[] tops = new string[] { };
            if (topCriteria != null)
                tops = topCriteria.Split(' ');

            using (var db = new DrdContext(_connString))
            {
                var mem = db.Members.FirstOrDefault(c => c.Id == memberId);

                var result =
                    (from c in db.PodCasts
                     where c.IsActive && (topCriteria == null || tops.All(x => c.Title.Contains(x)))
                     select new DtoPodCastLite
                     {
                         Id = c.Id,
                         Title = c.Title,
                         Duration = c.Duration,
                         Image = c.Image,
                         DateCreated = c.DateCreated,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }


        public IEnumerable<DtoPodCastLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, null, null);
        }

        public IEnumerable<DtoPodCastLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, order, null);
        }

        public IEnumerable<DtoPodCastLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            ApplConfigService.GenerateUniqueKeyLong();

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
                (from c in db.PodCasts
                 where c.CreatorId == creatorId && (topCriteria == null || tops.All(x => (c.Title).Contains(x)))
                 select new DtoPodCastLite
                 {
                     Id = c.Id,
                     Title = c.Title,
                     FileNameOri = c.FileNameOri,
                     Duration = c.Duration,
                     CreatorId = c.CreatorId,
                     IsActive = c.IsActive,
                     DateCreated = c.DateCreated,
                 }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService musvr = new MenuService();
                    foreach (DtoPodCastLite dl in result)
                    {
                        dl.Key = musvr.EncryptData(dl.Id);
                    }
                }

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
                    (from c in db.PodCasts
                     where c.CreatorId == creatorId && (topCriteria == null || tops.All(x => (c.Title).Contains(x)))
                     select new DtoPodCastLite
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();

                return result;

            }
        }

        public int Save(DtoPodCast prod)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            PodCast product;
            int result = 0;
            ApplConfigService asvr = new ApplConfigService();
            using (var db = new DrdContext(_connString))
            {
                if (prod.Id != 0)
                    product = db.PodCasts.FirstOrDefault(c => c.Id == prod.Id);
                else
                    product = new PodCast();

                product.Title = prod.Title;
                product.Descr = prod.Descr;
                product.Duration = prod.Duration;
                product.Image = prod.Image;
                product.AudioFileName = prod.AudioFileName;
                product.FileNameOri = prod.FileNameOri;
                product.IsActive = prod.IsActive;
                product.CreatorId = prod.CreatorId;

                if (prod.Id == 0)
                {
                    product.DateCreated = DateTime.Now;
                    db.PodCasts.Add(product);
                }
                else
                    product.DateUpdated = DateTime.Now;

                result = db.SaveChanges();

                return result;
            }

        }

    }
}
