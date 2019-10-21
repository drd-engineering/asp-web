using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Based.Core.Entity;
using DRD.Domain;
using System.Configuration;
using DRD.Core;
using System.Based.Core;

namespace DRD.Core
{
    public class DrDriveTypeService
    {
        private readonly string _connString;

        public DrDriveTypeService(string connString)
        {
            _connString = connString;
        }

        public DrDriveTypeService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public DtoDrDriveType GetById(int id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.DrDriveTypes
                     where c.Id == id
                     select new DtoDrDriveType
                     {
                         Id = c.Id,
                         Size = c.Size,
                         Price = c.Price,
                         ExpiryDay = c.ExpiryDay,
                     }).FirstOrDefault();
                if (result != null)
                {
                    result.StrSize = Ext.ToPrettySize(result.Size);
                }
                return result;
            }

        }

        public IEnumerable<DtoDrDriveType> GetAllExcluded()
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.DrDriveTypes
                     orderby c.Size
                     select new DtoDrDriveType
                     {
                         Id = c.Id,
                         Size = c.Size,
                         Price = c.Price,
                         ExpiryDay = c.ExpiryDay,

                     }).ToList();
                if (result != null)
                {
                    foreach (DtoDrDriveType t in result)
                    {
                        t.StrSize = Ext.ToPrettySize(t.Size);
                    }
                }
                return result;
            }

        }

    }
}
