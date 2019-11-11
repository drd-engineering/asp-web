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
    public class ProjectService
    {
        private readonly string _connString;
        private string _appZoneAccess;

        public ProjectService(string appZoneAccess, string connString)
        {
            _appZoneAccess = appZoneAccess;
            _connString = connString;
        }

        public ProjectService(string appZoneAccess)
        {
            _appZoneAccess = appZoneAccess;
            _connString = ConfigConstant.CONSTRING;
        }

        public ProjectService()
        {
            _connString = ConfigConstant.CONSTRING;
        }


        public DtoProject GetById(long id)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Projects
                     where c.Id == id
                     select new DtoProject
                     {
                         Id = c.Id,
                         Name = c.Name,
                         CompanyId = c.CompanyId,
                         Descr = c.Descr,
                         IsActive = c.IsActive,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         UserId = c.UserId,
                         Company = new DtoCompany
                         {
                             Id = c.Company.Id,
                             Name = c.Company.Name,
                         }
                     }).FirstOrDefault();

                return result;
            }
        }

        public IEnumerable<DtoProjectLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<DtoProjectLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<DtoProjectLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order, string criteria)
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
                    (from c in db.Projects
                     where c.CreatorId == creatorId &&  (topCriteria == null || tops.All(x => (c.Name).Contains(x)))
                     select new DtoProjectLite
                     {
                         Id = c.Id,
                         Name = c.Name,
                         CompanyId = c.CompanyId,
                         Descr = c.Descr,
                         CompanyName = c.Company.Name,
                         IsActive = c.IsActive,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         UserId = c.UserId,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService musvr = new MenuService();
                    foreach (DtoProjectLite dl in result)
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
                    (from c in db.Projects
                     where c.CreatorId == creatorId && (topCriteria == null || tops.All(x => (c.Name).Contains(x)))
                     select new DtoProjectLite
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();

                return result;

            }
        }

        public int Save(DtoProject prod)
        {
            Project product;
            using (var db = new DrdContext(_connString))
            {
                if (prod.Id != 0)
                {
                    product = db.Projects.FirstOrDefault(c => c.Id == prod.Id);
                }
                else {
                    product = new Project();
                }
                product.Name = prod.Name;
                product.Descr = prod.Descr;
                product.CompanyId = prod.CompanyId;
                product.IsActive = prod.IsActive;
                product.CreatorId = prod.CreatorId;
                product.UserId = prod.UserId;
                if (prod.Id == 0)
                {
                    product.DateCreated = DateTime.Now;
                    db.Projects.Add(product);
                }
                else
                    product.DateUpdated = DateTime.Now;

                var result = db.SaveChanges();


                return result;

            }

        }

    }
}
