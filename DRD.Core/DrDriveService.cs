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
    public class DrDriveService
    {
        private readonly string _connString;

        public DrDriveService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public DtoDrDrive GetById(long id)
        {

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.DrDrives
                     where c.Id == id
                     select new DtoDrDrive
                     {
                         Id = c.Id,
                         Descr = c.Descr,
                         FileName = c.FileName,
                         FileNameOri = c.FileNameOri,
                         ExtFile = c.ExtFile,
                         FileSize = c.FileSize,
                         FileFlag = c.FileFlag,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         MemberId = c.MemberId,
                         CxDownload = c.CxDownload,
                         DocumentId = c.DocumentId,
                         DocumentUploadId = c.DocumentUploadId,
                         MemberFolderId = c.MemberFolderId,
                     }).FirstOrDefault();

                return result;
            }
        }

        public DtoDrDrive GetByKeyId(string key)
        {
            DtoDrDrive doc = new DtoDrDrive();
            using (var db = new DrdContext(_connString))
            {
                long id = long.Parse(XEncryptionHelper.Decrypt(key));
                var result = db.DrDrives.FirstOrDefault(c => c.Id == id);
                if (result != null)
                {
                    doc.Id = result.Id;
                    doc.FileName = result.FileName;
                    doc.FileNameOri = result.FileNameOri;
                    doc.ExtFile = result.ExtFile;
                }

            }
            return doc;
        }

        public IEnumerable<DtoDrDrive> GetLiteAll(long memberId, long folderId, string topCriteria, int page, int pageSize)
        {
            return GetLiteAll(memberId, folderId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<DtoDrDrive> GetLiteAll(long memberId, long folderId, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteAll(memberId, folderId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<DtoDrDrive> GetLiteAll(long memberId, long folderId, string topCriteria, int page, int pageSize, string order, string criteria)
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
                (from c in db.DrDrives
                 where c.MemberId == memberId && c.MemberFolderId == folderId && (topCriteria == null || tops.All(x => (c.FileNameOri).Contains(x)))
                 select new DtoDrDrive
                 {
                     Id = c.Id,
                     Descr = c.Descr,
                     FileName = c.FileName,
                     FileNameOri = c.FileNameOri,
                     ExtFile = c.ExtFile,
                     FileSize = c.FileSize,
                     FileFlag = c.FileFlag,
                     DateCreated = c.DateCreated,
                     DateUpdated = c.DateUpdated,
                     MemberId = c.MemberId,
                     CxDownload = c.CxDownload,
                     DocumentId = c.DocumentId,
                     DocumentUploadId = c.DocumentUploadId,
                     MemberFolderId = c.MemberFolderId,
                 }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    foreach (DtoDrDrive dl in result)
                    {
                        dl.Key = XEncryptionHelper.Encrypt(dl.Id.ToString());
                    }
                }

                return result;

            }
        }

        public long GetLiteAllCount(long memberId, long folderId, string topCriteria)
        {
            return GetLiteAllCount(memberId, folderId, topCriteria, null);
        }
        public long GetLiteAllCount(long memberId, long folderId, string topCriteria, string criteria)
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
                    (from c in db.DrDrives
                     where c.MemberId == memberId && c.MemberFolderId == folderId && (topCriteria == null || tops.All(x => (c.FileNameOri).Contains(x)))
                     select new DtoDrDrive
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();

                return result;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prod"></param>
        /// <returns></returns>
        public long Save(DtoDrDrive prod)
        {
            DrDrive product;

            using (var db = new DrdContext(_connString))
            {
                if (prod.Id != 0)
                    product = db.DrDrives.FirstOrDefault(c => c.Id == prod.Id);
                else
                    product = new DrDrive();

                product.Descr = prod.Descr;
                product.FileName = prod.FileName;
                product.FileNameOri = prod.FileNameOri;
                product.ExtFile = prod.ExtFile;
                product.FileFlag = prod.FileFlag;
                product.FileSize = prod.FileSize;
                product.CxDownload = prod.CxDownload;
                product.MemberFolderId = prod.MemberFolderId;
                product.MemberId = prod.MemberId;
                product.DocumentId = prod.DocumentId;
                product.DocumentUploadId = prod.DocumentUploadId;

                if (prod.Id == 0)
                {
                    product.DateCreated = DateTime.Now;
                    db.DrDrives.Add(product);
                }
                else
                    product.DateUpdated = DateTime.Now;

                db.SaveChanges();

                return product.Id;
            }

        }

        public int Move(string docIds, long folderId)
        {
            long[] ids = docIds.Split(',').Select(Int64.Parse).ToArray();
            using (var db = new DrdContext(_connString))
            {
                var folder = db.MemberFolders.FirstOrDefault(c => c.Id == folderId);
                var docs = db.DrDrives.Where(c => ids.Contains(c.Id)).ToList();
                
                foreach (DrDrive doc in docs)
                {
                    doc.MemberFolderId = folderId;
                    doc.MemberId = folder.MemberId;
                }
                return db.SaveChanges();
            }
        }
        //public int Delete(long docId)
        //{
        //    using (var db = new DrdContext(_connString))
        //    {
        //        var doc = db.DrDrives.FirstOrDefault(c => c.Id == docId);
        //        if (doc == null)
        //            return 0;
        //        db.DrDrives.Remove(doc);
        //        return db.SaveChanges();
        //    }
        //}
        public int Delete(string docIds)
        {
            long[] ids = docIds.Split(',').Select(Int64.Parse).ToArray();
            using (var db = new DrdContext(_connString))
            {
                var docs = db.DrDrives.Where(c => ids.Contains(c.Id)).ToList();
                if (docs == null || docs.Count()==0)
                    return 0;
                db.DrDrives.RemoveRange(docs);
                return db.SaveChanges();
            }
        }
        public int SaveCxDownload(long docId)
        {
            using (var db = new DrdContext(_connString))
            {
                var doc = db.DrDrives.FirstOrDefault(c => c.Id == docId);
                doc.CxDownload++;
                db.SaveChanges();

                return doc.CxDownload;
            }
        }

        public JsonDrDriveCount GetCounting(long memberId)
        {
            using (var db = new DrdContext(_connString))
            {
                JsonDrDriveCount data = new JsonDrDriveCount();
                MemberPlanService psvr = new MemberPlanService();
                var plan = psvr.GetByMemberId(memberId);
                data.PlanStorageSize = plan.StorageSize + plan.StorageSizeAdd;

                data.Folders = new List<JsonDrDriveCount.FolderCounter>();
                var fsvr = new MemberFolderService();
                var folders = fsvr.GetDashboard(memberId);

                foreach (DtoMemberFolder folder in folders)
                {
                    JsonDrDriveCount.FolderCounter cx = new JsonDrDriveCount.FolderCounter();
                    cx.Folder = folder;

                    var drive = db.DrDrives.Where(c => c.MemberId == folder.MemberId && c.MemberFolderId == folder.Id).ToList();
                    if (drive != null)
                    {
                        cx.TotalFile = drive.Count();
                        if (cx.Folder.MemberId != 0)
                            cx.TotalSize = drive.Sum(c => c.FileSize);
                        cx.CxDownload = drive.Sum(c => c.CxDownload);
                    }
                    data.TotalStorageUsage += cx.TotalSize;
                    data.Folders.Add(cx);
                }

                return data;
            }
        }

    }
}
