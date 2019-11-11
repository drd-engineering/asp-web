using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Based.Core.Entity;
using DRD.Domain;
using System.Configuration;
using DRD.Service;
using System.Based.Core;

namespace DRD.Service
{
    public class SubscriptExtraTypeService
    {
        private readonly string _connString;

        public SubscriptExtraTypeService(string connString)
        {
            _connString = connString;
        }

        public SubscriptExtraTypeService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public DtoSubscriptExtraType GetById(int id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.SubscriptExtraTypes
                     where c.Id == id
                     select new DtoSubscriptExtraType
                     {
                         Id = c.Id,
                         Price = c.Price,
                         RotationCount = c.RotationCount,
                         FlowActivityCount = c.FlowActivityCount,
                         StorageSize = c.StorageSize,
                         DrDriveSize = c.DrDriveSize,
                     }).FirstOrDefault();

                if (result != null)
                {
                    result.StrStorageSize = Ext.ToPrettySize(result.StorageSize);
                    result.StrDrDriveSize = Ext.ToPrettySize(result.DrDriveSize);
                }

                return result;
            }

        }

        public IEnumerable<DtoSubscriptExtraType> GetAll()
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.SubscriptExtraTypes
                     select new DtoSubscriptExtraType
                     {
                         Id = c.Id,
                         Price = c.Price,
                         RotationCount = c.RotationCount,
                         FlowActivityCount = c.FlowActivityCount,
                         StorageSize = c.StorageSize,
                         DrDriveSize = c.DrDriveSize,
                     }).ToList();

                if (result != null)
                {
                    foreach (DtoSubscriptExtraType t in result)
                    {
                        t.StrStorageSize = Ext.ToPrettySize(t.StorageSize);
                        t.StrDrDriveSize = Ext.ToPrettySize(t.DrDriveSize);
                    }
                }

                return result;
            }

        }

    }
}
