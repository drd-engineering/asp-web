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
    public class NewsVideoService
    {
        private readonly string _connString;

        public NewsVideoService(string connString)
        {
            _connString = connString;
        }

        public NewsVideoService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public DtoNewsVideo GetById(long Id, long memberId)
        {
            MemberHitLogService log = new MemberHitLogService();
            log.Save(memberId, Id, ConfigConstant.enumDataHit.VIDEO);

            return GetById(Id);
        }

        public DtoNewsVideo GetById(long id)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.NewsVideos
                     where c.Id == id
                     select new DtoNewsVideo
                     {
                         Id = c.Id,
                         Code = c.Code,
                         Title = c.Title,
                         Descr = c.Descr,
                         CategoryId = c.CategoryId,
                         ChannelId = c.ChannelId,
                         ChannelTitle = c.ChannelTitle,
                         DatePublished = c.DatePublished,
                         IsActive = c.IsActive,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         CreatorId = c.CreatorId,
                     }).FirstOrDefault();


                return result;
            }
        }

        public IEnumerable<DtoNewsVideoLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, null, null);
        }

        public IEnumerable<DtoNewsVideoLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, order, null);
        }

        public IEnumerable<DtoNewsVideoLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order, string criteria)
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
                (from c in db.NewsVideos
                 where c.CreatorId == creatorId && (topCriteria == null || tops.All(x => (c.Title).Contains(x)))
                 select new DtoNewsVideoLite
                 {
                     Id = c.Id,
                     Code = c.Code,
                     Title = c.Title,
                     IsActive = c.IsActive,
                     CreatorId = c.CreatorId,
                     DateUpdated = c.DateUpdated,
                     DateCreated = c.DateCreated,
                 }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService musvr = new MenuService();
                    foreach (DtoNewsVideoLite dl in result)
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
                    (from c in db.NewsVideos
                     where c.CreatorId == creatorId && (topCriteria == null || tops.All(x => (c.Title).Contains(x)))
                     select new DtoNewsVideoLite
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();

                return result;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IEnumerable<DtoNewsVideoLite> GetLiteByTopCriteria(string topCriteria, int page, int pageSize, string order, string criteria)
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
                    (from c in db.NewsVideos
                     where c.IsActive && (topCriteria == null || tops.All(x => c.Title.Contains(x)))
                     select new DtoNewsVideoLite
                     {
                         Id = c.Id,
                         Code = c.Code,
                         Title = c.Title,
                         DatePublished = c.DatePublished,
                         DateCreated = c.DateCreated,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }

        public IEnumerable<DtoNewsVideoLite> GetLiteByTopCriteria(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
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
                    (from c in db.NewsVideos
                     where c.IsActive && (topCriteria == null || tops.All(x => c.Title.Contains(x)))
                     select new DtoNewsVideoLite
                     {
                         Id = c.Id,
                         Code = c.Code,
                         Title = c.Title,
                         DatePublished = c.DatePublished,
                         DateCreated = c.DateCreated,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prod"></param>
        /// <returns></returns>
        public int Save(DtoNewsVideo prod)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            NewsVideo product;
            int result = 0;
            ApplConfigService asvr = new ApplConfigService();
            using (var db = new DrdContext(_connString))
            {
                if (prod.Id != 0)
                    product = db.NewsVideos.FirstOrDefault(c => c.Id == prod.Id);
                else
                    product = new NewsVideo();

                product.Code = prod.Code;
                product.Title = prod.Title;
                product.Descr = prod.Descr;
                product.ChannelId = prod.ChannelId;
                product.ChannelTitle = prod.ChannelTitle;
                product.CategoryId = prod.CategoryId;
                product.DatePublished = prod.DatePublished;
                product.IsActive = prod.IsActive;
                product.CreatorId = prod.CreatorId;

                if (prod.Id == 0)
                {
                    product.DateCreated = DateTime.Now;
                    db.NewsVideos.Add(product);
                }
                else
                    product.DateUpdated = DateTime.Now;

                result = db.SaveChanges();

                return result;

            }

        }

    }
}
