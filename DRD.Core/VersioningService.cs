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
    public class VersioningService
    {
        private readonly string _connString;

        public VersioningService(string connString)
        {
            _connString = connString;
        }
        public VersioningService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public IEnumerable<DtoVersioning> GetAll()
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Versionings
                     orderby c.PackageName
                     select new DtoVersioning
                     {
                         Id = c.Id,
                         PackageName = c.PackageName,
                         VersionCode = c.VersionCode,
                         VersionName = c.VersionName,
                         Version = c.Version,
                     }).ToList();

                return result;
            }
        }

        public DtoVersioning GetById(int id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Versionings
                     where c.Id == id
                     select new DtoVersioning
                     {
                         Id = c.Id,
                         PackageName = c.PackageName,
                         VersionCode = c.VersionCode,
                         VersionName = c.VersionName,
                         Version = c.Version,
                     }).FirstOrDefault();

                return result;
            }
        }

        public DtoVersioning GetByPackageName(string packageName)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Versionings
                     where c.PackageName == packageName
                     select new DtoVersioning
                     {
                         Id = c.Id,
                         PackageName = c.PackageName,
                         VersionCode = c.VersionCode,
                         VersionName = c.VersionName,
                         Version = c.Version,
                     }).FirstOrDefault();

                return result;
            }
        }

        public DtoVersioning Save(DtoVersioning version)
        {
            Versioning data = new Versioning();
            using (var db = new DrdContext(_connString))
            {
                data.PackageName = version.PackageName;
                data.VersionCode = version.VersionCode;
                data.VersionName = version.VersionName;
                data.Version = version.Version;

                db.Versionings.Add(data);
                var result = db.SaveChanges();
                version.Id = data.Id;
                return version;
            }
        }

        public int Update(DtoVersioning version)
        {
            using (var db = new DrdContext(_connString))
            {
                var entity = db.Versionings.FirstOrDefault(c => c.Id == version.Id);
                if (entity == null) return 0;

                entity.PackageName = version.PackageName;
                entity.VersionCode = version.VersionCode;
                entity.VersionName = version.VersionName;
                entity.Version = version.Version;

                var result = db.SaveChanges();
                return result;
            }
        }
    }
}
