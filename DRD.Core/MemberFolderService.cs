using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Domain;
using System.Based.Core.Entity;
using System.Based.Core;

namespace DRD.Service
{
    public class MemberFolderService
    {
        private readonly string _connString;

        public MemberFolderService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public MemberFolderService(string connString)
        {
            _connString = connString;
        }

        public IEnumerable<DtoMemberFolder> GetAll(long memberId, long excludeId)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.MemberFolders
                     where c.Id != excludeId && (c.MemberId == memberId || (c.MemberId == 0 && c.FolderType == 1)) && c.FolderType == 1
                     orderby c.MemberId, c.Name
                     select new DtoMemberFolder
                     {
                         Id = c.Id,
                         Name = c.Name,
                         Descr = c.Descr,
                         MemberId = c.MemberId,
                         FolderType = c.FolderType,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                     }).ToList();

                return result;
            }
        }

        public IEnumerable<DtoMemberFolder> GetDashboard(long memberId)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.MemberFolders
                     where (c.MemberId == memberId || c.MemberId == 0)
                     orderby c.MemberId, c.Name
                     select new DtoMemberFolder
                     {
                         Id = c.Id,
                         Name = c.Name,
                         Descr = c.Descr,
                         MemberId = c.MemberId,
                         FolderType = c.FolderType,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                     }).ToList();

                return result;
            }
        }

        public DtoMemberFolder GetById(long id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.MemberFolders
                     where c.Id == id
                     select new DtoMemberFolder
                     {
                         Id = c.Id,
                         Name = c.Name,
                         Descr = c.Descr,
                         MemberId = c.MemberId,
                         FolderType = c.FolderType,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                     }).FirstOrDefault();

                return result;
            }
        }

        public long Save(DtoMemberFolder folder)
        {
            MemberFolder data = new MemberFolder();
            using (var db = new DrdContext(_connString))
            {
                if (folder.Id != 0)
                    data = db.MemberFolders.FirstOrDefault(c => c.Id == folder.Id);

                data.Name = folder.Name;
                data.Descr = folder.Descr;
                data.MemberId = folder.MemberId;
                data.FolderType = 0;
                if (folder.Id == 0)
                {
                    data.DateCreated = DateTime.Now;
                    db.MemberFolders.Add(data);
                }
                else
                    data.DateUpdated = DateTime.Now;
                db.SaveChanges();
                return data.Id;
            }
        }

        public int Remove(long memberId, long folderId)
        {
            using (var db = new DrdContext(_connString))
            {
                var doc = db.DrDrives.FirstOrDefault(c => c.MemberFolderId == folderId && c.MemberId == memberId);
                if (doc != null)
                    return 0;
                var folder = db.MemberFolders.FirstOrDefault(c => c.Id == folderId && c.MemberId == memberId);
                db.MemberFolders.Remove(folder);
                return db.SaveChanges();
            }
        }

    }

}
