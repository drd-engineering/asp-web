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
    public class NewsService
    {
        private readonly string _connString;

        public NewsService(string connString)
        {
            _connString = connString;
        }

        public NewsService()
        {
            _connString = ConfigConstant.CONSTRING;
        }


        public DtoNews GetById(long id)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.News
                     where c.Id == id
                     select new DtoNews
                     {
                         Id = c.Id,
                         Title = c.Title,
                         Descr = c.Descr,
                         IsActive = c.IsActive,
                         NewsType = c.NewsType,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         CreatorId = c.CreatorId,
                         NewsDetails =
                            (from x in c.NewsDetails
                             select new DtoNewsDetail
                             {
                                 Id = x.Id,
                                 Image = x.Image,
                                 Descr = x.Descr,
                             }).ToList(),
                     }).FirstOrDefault();

                GenericDataService gdsvr = new GenericDataService();
                result.Master.NewsTypes = gdsvr.GetNewsTypes();

                foreach (DtoNewsType mt in result.Master.NewsTypes)
                {
                    if ((result.NewsType & mt.BitValue) == mt.BitValue)
                    {
                        mt.IsChecked = true;
                    }
                }


                return result;
            }
        }

        public IEnumerable<DtoNewsLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, null, null);
        }

        public IEnumerable<DtoNewsLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, order, null);
        }

        public IEnumerable<DtoNewsLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order, string criteria)
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
                (from c in db.News
                 where c.CreatorId == creatorId && (topCriteria == null || tops.All(x => (c.Title).Contains(x)))
                 select new DtoNewsLite
                 {
                     Id = c.Id,
                     Title = c.Title,
                     CreatorId = c.CreatorId,
                     IsActive = c.IsActive,
                     NewsType = c.NewsType,
                     DateCreated = c.DateCreated,
                 }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService musvr = new MenuService();
                    foreach (DtoNewsLite dl in result)
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
                    (from c in db.News
                     where c.CreatorId == creatorId && (topCriteria == null || tops.All(x => (c.Title).Contains(x)))
                     select new DtoNewsLite
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();

                return result;

            }
        }

        public IEnumerable<DtoNewsLite> GetLiteByQuery(string query, int page, int pageSize, string order, string criteria)
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
                    (from c in db.News
                     where c.IsActive
                     select new DtoNewsLite
                     {
                         Id = c.Id,
                         Title = c.Title,
                         Descr = c.Descr.Substring(0, 150) + "...",
                         NewsType = c.NewsType,
                         CreatorId = c.CreatorId,
                         DateCreated = c.DateCreated,
                         Image = c.NewsDetails.FirstOrDefault().Image,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="type"></param>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IEnumerable<DtoNewsLite> GetLiteByTopCriteria(long memberId, int type, string topCriteria, int page, int pageSize, string order, string criteria)
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
                    (from c in db.News
                     where c.IsActive && (c.NewsType & type) == type && (topCriteria == null || tops.All(x => c.Title.Contains(x)))
                     select new DtoNewsLite
                     {
                         Id = c.Id,
                         Title = c.Title,
                         Descr = c.Descr.Substring(0, 150) + "...",
                         NewsType = c.NewsType,
                         CreatorId = c.CreatorId,
                         DateCreated = c.DateCreated,
                         Image = c.NewsDetails.FirstOrDefault().Image,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }

        public IEnumerable<DtoNewsLite> GetLiteByTopCriteria(long memberId, string types, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "DateCreated desc";

            int[] atype = Array.ConvertAll(types.Split(','), int.Parse);

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
                    (from c in db.News
                     where c.IsActive && atype.Contains(c.NewsType) && (topCriteria == null || tops.All(x => c.Title.Contains(x)))
                     select new DtoNewsLite
                     {
                         Id = c.Id,
                         Title = c.Title,
                         Descr = c.Descr.Substring(0, 150) + "...",
                         NewsType = c.NewsType,
                         CreatorId = c.CreatorId,
                         DateCreated = c.DateCreated,
                         Image = c.NewsDetails.FirstOrDefault().Image,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }

        public IEnumerable<DtoNewsLite> GetLiteByTopCriteria(int type, string topCriteria, int page, int pageSize, string order, string criteria)
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
                    (from c in db.News
                     where c.IsActive && (c.NewsType & type) == type && (topCriteria == null || tops.All(x => c.Title.Contains(x)))
                     select new DtoNewsLite
                     {
                         Id = c.Id,
                         Title = c.Title,
                         Descr = c.Descr.Substring(0, 150) + "...",
                         NewsType = c.NewsType,
                         CreatorId = c.CreatorId,
                         DateCreated = c.DateCreated,
                         Image = c.NewsDetails.FirstOrDefault().Image,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }

        public IEnumerable<DtoNewsLite> GetLiteByTopCriteria(string types, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "DateCreated desc";

            int[] atype = Array.ConvertAll(types.Split(','), int.Parse);

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
                    (from c in db.News
                     where c.IsActive && atype.Contains(c.NewsType) && (topCriteria == null || tops.All(x => c.Title.Contains(x)))
                     select new DtoNewsLite
                     {
                         Id = c.Id,
                         Title = c.Title,
                         Descr = c.Descr.Substring(0, 150) + "...",
                         NewsType = c.NewsType,
                         CreatorId = c.CreatorId,
                         DateCreated = c.DateCreated,
                         Image = c.NewsDetails.FirstOrDefault().Image,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="prod"></param>
        /// <returns></returns>
        public int Save(DtoNews prod)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            News product;
            int result = 0;
            ApplConfigService asvr = new ApplConfigService();
            using (var db = new DrdContext(_connString))
            {
                if (prod.Id != 0)
                    product = db.News.FirstOrDefault(c => c.Id == prod.Id);
                else
                    product = new News();

                product.Title = prod.Title;
                product.Descr = prod.Descr;
                product.NewsType = prod.NewsType;
                product.IsActive = prod.IsActive;
                product.CreatorId = prod.CreatorId;

                if (prod.Id == 0)
                {
                    product.DateCreated = DateTime.Now;
                    db.News.Add(product);
                }
                else
                    product.DateUpdated = DateTime.Now;

                result = db.SaveChanges();


                //
                // prepare data images
                //
                var cxold = db.NewsDetails.Count(c => c.NewsId == product.Id);
                var cxnew = prod.NewsDetails.Count();
                if (cxold < cxnew)
                {
                    var ep = prod.NewsDetails.ElementAt(0); // get 1 data for sample
                    for (var x = cxold; x < cxnew; x++)
                    {
                        NewsDetail aii = new NewsDetail();
                        aii.NewsId = product.Id;
                        aii.Image = ep.Image;
                        aii.Descr = ep.Descr;
                        db.NewsDetails.Add(aii);
                    }
                    db.SaveChanges();
                }
                else if (cxold > cxnew)
                {
                    var dremove = db.NewsDetails.Where(c => c.NewsId == product.Id).Take(cxold - cxnew).ToList();
                    db.NewsDetails.RemoveRange(dremove);
                    db.SaveChanges();
                }

                // save images
                var dnew = db.NewsDetails.Where(c => c.NewsId == product.Id).ToList();
                int v = 0;
                foreach (NewsDetail d in dnew)
                {
                    var epos = prod.NewsDetails.ElementAt(v);
                    d.Image = epos.Image;
                    d.Descr = epos.Descr;
                    v++;
                }
                db.SaveChanges();

                return result;

            }

        }

        public DtoNews GetById(long Id, long memberId)
        {
            MemberHitLogService log = new MemberHitLogService();
            log.Save(memberId, Id, ConfigConstant.enumDataHit.NEWS);

            return GetById(Id);
        }

        public DtoNewsMaster GetMaster()
        {
            DtoNewsMaster masters = new DtoNewsMaster();

            GenericDataService csvr = new GenericDataService();
            masters.NewsTypes = csvr.GetNewsTypes();

            return masters;

        }

    }
}
