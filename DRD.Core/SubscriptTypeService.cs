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
    public class SubscriptTypeService
    {
        private readonly string _connString;

        public SubscriptTypeService(string connString)
        {
            _connString = connString;
        }

        public SubscriptTypeService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public DtoSubscriptType GetById(int id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.SubscriptTypes
                     where c.Id == id
                     select new DtoSubscriptType
                     {
                         Id = c.Id,
                         TypeCode = c.TypeCode,
                         ClassName = c.ClassName,
                         Descr = c.Descr,
                         Price = c.Price,
                         PriceUnitCode = c.PriceUnitCode,
                         PriceUnitDescr = c.PriceUnitDescr,
                         RotationCount = c.RotationCount,
                         RotationPrice = c.RotationPrice,
                         FlowActivityCount = c.FlowActivityCount,
                         FlowActivityPrice = c.FlowActivityPrice,
                         StorageSize = c.StorageSize,
                         StoragePrice = c.StoragePrice,
                         ExpiryDocDay = c.ExpiryDocDay,
                         DrDrivePrice = c.DrDrivePrice,
                         DrDriveSize = c.DrDriveSize,
                         PackageExpiryDay = c.PackageExpiryDay,
                     }).FirstOrDefault();

                if (result != null)
                {
                    result.StrStorageSize = Ext.ToPrettySize(result.StorageSize);
                    result.StrDrDriveSize = Ext.ToPrettySize(result.DrDriveSize);
                }
                return result;
            }

        }

        public IEnumerable<DtoSubscriptType> GetAllExcluded(int excludeId)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.SubscriptTypes
                         //where c.Id != excludeId
                     where !c.TypeCode.Equals("INIT")
                     select new DtoSubscriptType
                     {
                         Id = c.Id,
                         TypeCode = c.TypeCode,
                         ClassName = c.ClassName,
                         Descr = c.Descr,
                         Price = c.Price,
                         PriceUnitCode = c.PriceUnitCode,
                         PriceUnitDescr = c.PriceUnitDescr,
                         RotationCount = c.RotationCount,
                         RotationPrice = c.RotationPrice,
                         FlowActivityCount = c.FlowActivityCount,
                         FlowActivityPrice = c.FlowActivityPrice,
                         StorageSize = c.StorageSize,
                         StoragePrice = c.StoragePrice,
                         ExpiryDocDay = c.ExpiryDocDay,
                         DrDrivePrice = c.DrDrivePrice,
                         DrDriveSize = c.DrDriveSize,
                         PackageExpiryDay = c.PackageExpiryDay,

                     }).ToList();

                if (result != null)
                {
                    foreach (DtoSubscriptType t in result)
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
