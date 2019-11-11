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
    public class MemberPlanService
    {
        private readonly string _connString;

        public MemberPlanService(string connString)
        {
            _connString = connString;
        }

        public MemberPlanService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public DtoMemberPlan GetById(int id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.MemberPlans
                     where c.Id == id
                     select new DtoMemberPlan
                     {
                         Id = c.Id,
                         MemberId = c.MemberId,
                         SubscriptTypeId = c.SubscriptTypeId,
                         Price = c.Price,
                         PriceUnitCode = c.PriceUnitCode,
                         PriceUnitDescr = c.PriceUnitDescr,
                         RotationCount = c.RotationCount,
                         RotationPrice = c.RotationPrice,
                         RotationCountAdd = c.RotationCountAdd,
                         RotationCountUsed = c.RotationCountUsed,
                         FlowActivityCount = c.FlowActivityCount,
                         FlowActivityPrice = c.FlowActivityPrice,
                         FlowActivityCountAdd = c.FlowActivityCountAdd,
                         FlowActivityCountUsed = c.FlowActivityCountUsed,
                         StorageSize = c.StorageSize,
                         StoragePrice = c.StoragePrice,
                         StorageSizeAdd = c.StorageSizeAdd,
                         StorageSizeUsed = c.StorageSizeUsed,
                         DrDriveSize = c.DrDriveSize,
                         DrDrivePrice = c.DrDrivePrice,
                         DrDriveSizeAdd = c.DrDriveSizeAdd,
                         DrDriveSizeUsed = c.DrDriveSizeUsed,
                         ExpiryDocDay = c.ExpiryDocDay,
                         PackageExpiryDay = c.PackageExpiryDay,
                         ValidPackage = c.ValidPackage,
                         ValidDrDrive = c.ValidDrDrive,
                         IsDefault = c.IsDefault,
                         SubscriptType = new DtoSubscriptType
                         {
                             TypeCode = c.SubscriptType.TypeCode,
                             ClassName = c.SubscriptType.ClassName,

                         }
                     }).FirstOrDefault();

                return result;
            }

        }
        public DtoMemberPlan GetByMemberId(long id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.MemberPlans
                     where c.MemberId == id && c.IsDefault
                     select new DtoMemberPlan
                     {
                         Id = c.Id,
                         MemberId = c.MemberId,
                         SubscriptTypeId = c.SubscriptTypeId,
                         Price = c.Price,
                         PriceUnitCode = c.PriceUnitCode,
                         PriceUnitDescr = c.PriceUnitDescr,
                         RotationCount = c.RotationCount,
                         RotationPrice = c.RotationPrice,
                         RotationCountAdd = c.RotationCountAdd,
                         RotationCountUsed = c.RotationCountUsed,
                         FlowActivityCount = c.FlowActivityCount,
                         FlowActivityPrice = c.FlowActivityPrice,
                         FlowActivityCountAdd = c.FlowActivityCountAdd,
                         FlowActivityCountUsed = c.FlowActivityCountUsed,
                         StorageSize = c.StorageSize,
                         StoragePrice = c.StoragePrice,
                         StorageSizeAdd = c.StorageSizeAdd,
                         StorageSizeUsed = c.StorageSizeUsed,
                         DrDriveSize = c.DrDriveSize,
                         DrDrivePrice = c.DrDrivePrice,
                         DrDriveSizeAdd = c.DrDriveSizeAdd,
                         DrDriveSizeUsed = c.DrDriveSizeUsed,
                         ExpiryDocDay = c.ExpiryDocDay,
                         PackageExpiryDay = c.PackageExpiryDay,
                         ValidPackage = c.ValidPackage,
                         ValidDrDrive = c.ValidDrDrive,
                         IsDefault = c.IsDefault,

                         SubscriptType = new DtoSubscriptType
                         {
                             TypeCode = c.SubscriptType.TypeCode,
                             ClassName = c.SubscriptType.ClassName,

                         }
                     }).FirstOrDefault();

                if (result != null)
                {
                    result.StrStorageSize = Ext.ToPrettySize(result.StorageSize);
                    result.StrStorageSizeAdd = Ext.ToPrettySize(result.StorageSizeAdd);
                    result.StrStorageSizeUsed = Ext.ToPrettySize(result.StorageSizeUsed);
                    result.StrStorageSizeBal = Ext.ToPrettySize(result.StorageSize + result.StorageSizeAdd - result.StorageSizeUsed);

                    result.StrDrDriveSize = Ext.ToPrettySize(result.DrDriveSize);
                    result.StrDrDriveSizeAdd = Ext.ToPrettySize(result.DrDriveSizeAdd);
                    result.StrDrDriveSizeUsed = Ext.ToPrettySize(result.DrDriveSizeUsed);
                    result.StrDrDriveSizeBal = Ext.ToPrettySize(result.DrDriveSize + result.DrDriveSizeAdd - result.DrDriveSizeUsed);
                }

                return result;
            }

        }

        public long Save(long memberId, int subscriptTypeId)
        {
            using (var db = new DrdContext(_connString))
            {

                var subscr = db.SubscriptTypes.FirstOrDefault(c => c.Id == subscriptTypeId);
                long drdriveSize = 0;
                decimal drdrivePrice = 0;
                long drdriveSizeAdd = 0;
                long drdriveSizeUsed = 0;
                DateTime? validDrDrive = null;
                var drDriveExpiryDay = 0;
                var curPlan = db.MemberPlans.FirstOrDefault(c => c.MemberId == memberId && c.IsDefault);
                if (curPlan != null)
                {
                    drdriveSize = curPlan.DrDriveSize;
                    drdrivePrice = curPlan.DrDrivePrice;
                    drdriveSizeAdd = curPlan.DrDriveSizeAdd;
                    drdriveSizeUsed = curPlan.DrDriveSizeUsed;
                    validDrDrive = curPlan.ValidDrDrive;
                    drDriveExpiryDay = curPlan.DrDriveExpiryDay;
                    curPlan.IsDefault = false;
                    db.SaveChanges();
                }

                var plan = new MemberPlan();
                plan.MemberId = memberId;
                plan.SubscriptTypeId = subscriptTypeId;
                plan.Price = subscr.Price;
                plan.PriceUnitCode = subscr.PriceUnitCode;
                plan.PriceUnitDescr = subscr.PriceUnitDescr;
                plan.RotationCount = subscr.RotationCount;
                plan.RotationPrice = subscr.RotationPrice;
                plan.FlowActivityCount = subscr.FlowActivityCount;
                plan.FlowActivityPrice = subscr.FlowActivityPrice;
                plan.StorageSize = subscr.StorageSize;
                plan.StoragePrice = subscr.StoragePrice;
                plan.DrDriveSize = drdriveSize;
                plan.DrDrivePrice = drdrivePrice;
                plan.DrDriveSizeAdd = drdriveSizeAdd;
                plan.DrDriveSizeUsed = drdriveSizeUsed;
                plan.ExpiryDocDay = subscr.ExpiryDocDay;
                plan.PackageExpiryDay = subscr.PackageExpiryDay;
                plan.DrDriveExpiryDay = drDriveExpiryDay;
                plan.ValidPackage = DateTime.Now.AddDays(subscr.PackageExpiryDay);
                plan.ValidDrDrive = validDrDrive;
                plan.IsDefault = true;
                plan.UserId = "SYST";
                plan.DateCreated = DateTime.Now;
                db.MemberPlans.Add(plan);
                db.SaveChanges();
                return plan.Id;
            }
        }

        public long SaveExtra(long memberId, int planId)
        {
            using (var db = new DrdContext(_connString))
            {

                var extra = db.SubscriptExtraTypes.FirstOrDefault(c => c.Id == planId);
                var plan = db.MemberPlans.FirstOrDefault(c => c.MemberId == memberId && c.IsDefault);

                plan.RotationCountAdd += extra.RotationCount;
                plan.FlowActivityCountAdd += extra.FlowActivityCount;
                plan.StorageSizeAdd += extra.StorageSize;
                plan.DrDriveSizeAdd += extra.DrDriveSize;

                plan.DateUpdated = DateTime.Now;
                db.SaveChanges();
                return plan.Id;
            }
        }

        public long SaveDrDrive(long memberId, int driveId)
        {
            using (var db = new DrdContext(_connString))
            {

                var drive = db.DrDriveTypes.FirstOrDefault(c => c.Id == driveId);
                var plan = db.MemberPlans.FirstOrDefault(c => c.MemberId == memberId && c.IsDefault);

                plan.DrDriveSize = drive.Size;
                plan.DrDrivePrice = drive.Price;

                plan.ValidDrDrive = DateTime.Now.AddDays(drive.ExpiryDay);
                plan.DateUpdated = DateTime.Now;
                db.SaveChanges();
                return plan.Id;
            }
        }

        public long DeductPlan(long memberId, int size, string sizeName)
        {
            using (var db = new DrdContext(_connString))
            {
                var plan = db.MemberPlans.FirstOrDefault(c => c.MemberId == memberId && c.IsDefault);
                if (sizeName.Equals("ROTATION"))
                    plan.RotationCountUsed += size;
                else if (sizeName.Equals("DRDRIVE"))
                    plan.DrDriveSizeUsed += size;
                else if (sizeName.Equals("DOCUMENT"))
                    plan.StorageSizeUsed += size;

                plan.DateUpdated = DateTime.Now;
                db.SaveChanges();
                return plan.Id;
            }
        }


    }
}
